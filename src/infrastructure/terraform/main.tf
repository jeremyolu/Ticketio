resource "azurerm_resource_group" "ticketio" {
  name     = "rg-${var.project_name}"
  location = var.location

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_storage_account" "str" {
  name                     = "strticketio"
  resource_group_name      = azurerm_resource_group.ticketio.name
  location                 = azurerm_resource_group.ticketio.location
  account_tier             = "Standard"
  account_replication_type = "LRS"

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_container_registry" "acr" {
  name                = "registryTicketio"
  resource_group_name = azurerm_resource_group.ticketio.name
  location            = azurerm_resource_group.ticketio.location
  sku                 = "Basic"
  admin_enabled       = false

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_container_app_environment" "acaenv" {
  name                = "cae-${var.project_name}"
  location            = azurerm_resource_group.ticketio.location
  resource_group_name = azurerm_resource_group.ticketio.name

  workload_profile {
    name                  = "Consumption"
    workload_profile_type = "Consumption"
  }

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_user_assigned_identity" "aca" {
  name                = "id-${var.project_name}-aca"
  resource_group_name = azurerm_resource_group.ticketio.name
  location            = azurerm_resource_group.ticketio.location

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_role_assignment" "aca_acr_pull" {
  scope                = azurerm_container_registry.acr.id
  role_definition_name = "AcrPull"
  principal_id         = azurerm_user_assigned_identity.aca.principal_id
}

resource "azurerm_container_app" "auth" {
  name                         = "ca-${var.project_name}-auth-api"
  container_app_environment_id = azurerm_container_app_environment.acaenv.id
  resource_group_name          = azurerm_resource_group.ticketio.name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  depends_on = [
    azurerm_role_assignment.aca_acr_pull
  ]

  identity {
    type = "UserAssigned"

    identity_ids = [
      azurerm_user_assigned_identity.aca.id
    ]
  }

  registry {
    server   = azurerm_container_registry.acr.login_server
    identity = azurerm_user_assigned_identity.aca.id
  }

  ingress {
    external_enabled = false
    target_port      = 8080

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  secret {
    name  = "jwt-key"
    value = var.jwt_key
  }

  secret {
    name  = "ticketio-db"
    value = var.auth_db_connection_string
  }

  template {
    container {
      name   = "auth-api"
      image  = "${azurerm_container_registry.acr.login_server}/ticketio-auth-api:${var.latest_auth_image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name        = "Jwt__Key"
        secret_name = "jwt-key"
      }

      env {
        name        = "ConnectionStrings__TicketioDb"
        secret_name = "ticketio-db"
      }

      env {
        name  = "Jwt__Issuer"
        value = var.jwt_issuer
      }

      env {
        name  = "Jwt__Audience"
        value = var.jwt_audience
      }

      env {
        name  = "Jwt__AccessTokenExpiryMinutes"
        value = "15"
      }

      env {
        name  = "Jwt__RefreshTokenExpiryDays"
        value = "7"
      }
    }

    min_replicas = 0
    max_replicas = 1
  }

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}

resource "azurerm_container_app" "api" {
  name                         = "ca-${var.project_name}-api"
  container_app_environment_id = azurerm_container_app_environment.acaenv.id
  resource_group_name          = azurerm_resource_group.ticketio.name
  revision_mode                = "Single"
  workload_profile_name        = "Consumption"

  depends_on = [
    azurerm_role_assignment.aca_acr_pull
  ]

  identity {
    type = "UserAssigned"

    identity_ids = [
      azurerm_user_assigned_identity.aca.id
    ]
  }

  registry {
    server   = azurerm_container_registry.acr.login_server
    identity = azurerm_user_assigned_identity.aca.id
  }

  ingress {
    external_enabled = true
    target_port      = 8080

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  template {
    container {
      name   = "api"
      image  = "${azurerm_container_registry.acr.login_server}/ticketio-api:${var.latest_api_image_tag}"
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "ReverseProxy__Clusters__AuthCluster__Destinations__AuthApi__Address"
        value = "https://${azurerm_container_app.auth.ingress[0].fqdn}/"
      }
    }

    min_replicas = 0
    max_replicas = 1
  }
}
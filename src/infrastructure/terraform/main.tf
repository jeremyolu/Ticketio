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

  tags = {
    project    = var.project_name
    managed_by = "terraform"
  }
}
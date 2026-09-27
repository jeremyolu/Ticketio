output "resource_group_name" {
  value = azurerm_resource_group.ticketio.name
}

output "resource_group_location" {
  value = azurerm_resource_group.ticketio.location
}

output "storage_account_name" {
  value = azurerm_storage_account.str.name
}

output "container_registry_name" {
  value = azurerm_container_registry.acr.name
}

output "container_registry_login_server" {
  value = azurerm_container_registry.acr.login_server
}
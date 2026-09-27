terraform {
  backend "azurerm" {
    resource_group_name  = "rg-ticketio-tfstate"
    storage_account_name = "sttfstateticketio"
    container_name       = "tfstate"
    key                  = "ticketio.dev.tfstate"
  }
}
#include <Uefi.h>
#include <Library/UefiBootServicesTableLib.h>
#include <Library/PrintLib.h>

EFI_STATUS
EFIAPI
UefiMain(
    IN EFI_HANDLE ImageHandle,
    IN EFI_SYSTEM_TABLE* SystemTable
) {
    Print(L"Hello, this is a test message!\n");
    return EFI_SUCCESS;
}

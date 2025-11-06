; ============================================================
; CaYa Kernel v1.0.0 - 32-bit Protected Mode Kernel
; Loaded at 0x10000 by bootloader
; ============================================================

bits 32
org 0x10000

kernel_start:
    ; Set up data segments
    mov ax, 0x10
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov fs, ax
    mov gs, ax
    
    ; Set up stack
    mov esp, 0x9C00
    
    ; Clear screen
    mov edi, 0xB8000
    mov ecx, 2000
    mov ax, 0x0720
.clear:
    mov word [edi], ax
    add edi, 2
    loop .clear
    
    ; Display test message
    mov edi, 0xB8000
    mov byte [edi], 'K'
    mov byte [edi + 1], 0x0F  ; White on black
    
    ; Halt
    cli
    hlt
    jmp $

; Strings
kernel_title: db "CaYaDev Kernel", 0

; Padding to 16KB
times 16384 - ($ - $$) db 0
    db "=== CaYaDev Recovery Kernel ===", 0

kernel_info:
    db "32-bit Protected Mode Active", 0

kernel_status:
    db "Press CTRL+ALT+DEL to restart", 0

; ============================================================
; FILL TO 16KB
; ============================================================

times (0x10000 + 0x4000) - ($ - $$) db 0

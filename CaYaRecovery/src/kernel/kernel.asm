; ============================================================
; CaYa Kernel v1.0.0 - 32-bit Protected Mode Kernel
; Bootloader'dan 0x10000'de çalışır
; ============================================================

bits 32
org 0x10000

kernel_start:
    ; Stack setup
    mov esp, 0x9C00
    
    ; Clear screen (fill video buffer with spaces)
    mov edi, 0xB8000
    mov ecx, 2000
    mov ax, 0x0720  ; Space char with white on black
.clear:
    mov word [edi], ax
    add edi, 2
    loop .clear
    
    ; Print title at 0xB8000 (line 0)
    mov edi, 0xB8000
    mov esi, kernel_title
    call pm_print_string
    
    ; Print info at line 2 (offset 160 = 80*2)
    mov edi, 0xB8000 + 160
    mov esi, kernel_info
    call pm_print_string
    
    ; Print status at line 4 (offset 320 = 160*2)
    mov edi, 0xB8000 + 320
    mov esi, kernel_status
    call pm_print_string
    
    ; Halt
    cli
    jmp $

; Print string in protected mode
; ESI = string pointer, EDI = video buffer address
pm_print_string:
    push edi
    push esi
.loop:
    mov al, byte [esi]
    test al, al
    jz .done
    
    mov byte [edi], al           ; Character
    mov byte [edi + 1], 0x07     ; Attribute (white on black)
    
    add edi, 2
    inc esi
    jmp .loop
    
.done:
    pop esi
    pop edi
    ret

; ============================================================
; DATA SECTION
; ============================================================

kernel_title:
    db "=== CaYaDev Recovery Kernel ===", 0

kernel_info:
    db "32-bit Protected Mode Active", 0

kernel_status:
    db "Press CTRL+ALT+DEL to restart", 0

; ============================================================
; FILL TO 16KB
; ============================================================

times (0x10000 + 0x4000) - ($ - $$) db 0

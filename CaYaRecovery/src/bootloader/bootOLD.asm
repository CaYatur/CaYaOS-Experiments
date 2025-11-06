; CaYa Bootloader v1.0.0 - Custom x86 MBR
; El-Torito bootable CD için

bits 16
org 0x7C00

; Boot sector entry
jmp short _start
nop

; BPB (BIOS Parameter Block) - CD boot için minimal BPB
; Offset 3-61 (59 bytes)
db "CAYADOS"           ; 8 bytes OEM ID
dw 2048              ; Bytes per sector (CD standard)
db 1                 ; Sectors per cluster
dw 0                 ; Reserved sectors
db 0                 ; Number of FATs
dw 0                 ; Root directory entries
dw 0                 ; Total sectors (small)
db 0xF8              ; Media descriptor (removable)
dw 0                 ; Sectors per FAT
dw 0                 ; Sectors per track
dw 0                 ; Heads
dd 0                 ; Hidden sectors
dd 0                 ; Total sectors (large)
db 0x80              ; Drive number (removable)
db 0                 ; Reserved
db 0x29              ; Boot signature
dd 0xFFFFFFFF        ; Serial number
db "CAYARECOVERY"    ; Volume label (11 bytes)
db "FAT     "        ; File system (8 bytes)

_start:
    ; Clear registers
    xor ax, ax
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov sp, 0x7C00
    
    ; Call boot menu (handles ENTER/ESC)
    call clear_screen
    call show_boot_menu
    ; If we reach here, ENTER was pressed
    jmp jump_to_kernel
    
    ; If ESC pressed, int 0x19 will restart BIOS boot

clear_screen:
    mov ax, 0x0700
    mov bh, 0x07
    mov cx, 0x0000
    mov dx, 0x184F
    int 0x10
    
    mov ah, 0x02
    mov bh, 0x00
    mov dx, 0x0000
    int 0x10
    ret

print_string:
    pusha
.loop:
    mov al, [si]
    cmp al, 0
    je .done
    mov ah, 0x0E
    mov bh, 0x00
    int 0x10
    inc si
    jmp .loop
.done:
    popa
    ret

show_boot_menu:
    mov si, msg_title
    call print_string
    mov si, msg_info
    call print_string
    mov si, msg_press
    call print_string
    mov si, msg_esc
    call print_string
.wait:
    mov ah, 0x00
    int 0x16
    
    cmp al, 13           ; ENTER
    je .boot_cayadev
    
    cmp al, 27           ; ESC
    je .boot_fallback
    
    jmp .wait
    
    .boot_cayadev:
    mov si, msg_go
    call print_string
    ret
    
.boot_fallback:
    mov si, msg_fallback
    call print_string
    ; Reboot to next device (BIOS handles it)
    int 0x19
    jmp $

jump_to_kernel:
    ; Setup GDT
    lgdt [gdt_descriptor]
    
    ; Set PE bit in CR0
    mov eax, cr0
    or eax, 1
    mov cr0, eax
    
    ; Clear instruction cache with far jump
    jmp 0x08:pm_start

bits 32
pm_start:
    ; Setup segments
    mov ax, 0x10
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov esp, 0x9000
    
    ; Clear screen
    mov edi, 0xB8000
    mov ecx, 2000
    mov ax, 0x0720
.cls:
    mov word [edi], ax
    add edi, 2
    loop .cls
    
    ; Print message
    mov edi, 0xB8000
    mov esi, kernel_msg
    call pm_print
    
    ; Halt
    cli
    hlt
    jmp $

pm_print:
    push edi
.lp:
    mov al, [esi]
    test al, al
    jz .dn
    mov [edi], al
    mov byte [edi+1], 0x07
    add edi, 2
    inc esi
    jmp .lp
.dn:
    pop edi
    ret

bits 16
kernel_msg:
    db "=== CaYaDev Kernel Running ===", 0

enter_protected_mode:
    ; Setup GDT
    lgdt [gdt_descriptor]
    
    ; Set PE bit in CR0
    mov eax, cr0
    or eax, 1
    mov cr0, eax
    
    ; Far jump to PM code
    jmp 0x08:pm_entry

; GDT
align 8
gdt:
    dd 0
    dd 0
    
    dw 0xFFFF
    dw 0x0000
    db 0x00
    db 0x9A
    db 0xCF
    db 0x00
    
    dw 0xFFFF
    dw 0x0000
    db 0x00
    db 0x92
    db 0xCF
    db 0x00

gdt_descriptor:
    dw gdt_descriptor - gdt - 1
    dd gdt

bits 32
pm_entry:
    mov ax, 0x10
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov esp, 0x7C00
    jmp 0x10000
    jmp $

bits 16

; Messages (minimal)
msg_title:
    db 0x0D, 0x0A, "CaYaDev Recovery v1.0.0", 0x0D, 0x0A, 0

msg_info:
    db "32/64-bit Compatible", 0x0D, 0x0A, 0

msg_press:
    db "ENTER: Boot CaYaDev", 0x0D, 0x0A, 0

msg_esc:
    db "ESC: Boot Next Device", 0x0D, 0x0A, 0

msg_go:
    db "Loading CaYaDev...", 0x0D, 0x0A, 0

msg_fallback:
    db "Booting next device...", 0x0D, 0x0A, 0

; Boot signature
times 510 - ($ - $$) db 0
dw 0xAA55

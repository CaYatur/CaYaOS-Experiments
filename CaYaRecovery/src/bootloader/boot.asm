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
    ; Debug: Print before PM
    mov si, msg_pm_start
    call print_string
    
    ; Disable interrupts
    cli
    
    ; Setup Real Mode stack first
    mov ax, 0x0000
    mov ss, ax
    mov esp, 0x7000
    
    ; Load GDT
    lgdt [gdt_descriptor]
    
    ; Set PE bit in CR0 (enable Protected Mode)
    mov eax, cr0
    or eax, 0x00000001
    mov cr0, eax
    
    ; Flush instruction cache with far jump
    ; This switches to 32-bit mode with segment 0x08 (code)
    jmp dword 0x08:pm_start

bits 32
pm_start:
    ; Setup segments
    mov ax, 0x10
    mov ds, ax
    mov es, ax
    mov ss, ax
    mov esp, 0x9000
    
    ; Write 'K' char at 0xB8000
    mov dword [0xB8000], 0x0741

    ; Write 'K' and halt
    mov eax, 0xB8000
    mov byte [eax], 'K'
    mov byte [eax+1], 0x0F
    
    ; Halt
    cli
    hlt
    jmp $

bits 16

; GDT - Global Descriptor Table
align 8
gdt_start:
    ; Null descriptor (required by x86)
    dq 0
    
    ; Code segment (selector 0x08)
    ; Flags: Present, DPL=0, Code (Readable)
    dw 0xFFFF        ; Limit 0:15
    dw 0x0000        ; Base 0:15
    db 0x00          ; Base 16:23
    db 0x9B          ; P=1 DPL=0 S=1 Type=3 (Code: execute/read)
    db 0xCF          ; G=1 D=1 (32-bit granular)
    db 0x00          ; Base 24:31
    
    ; Data segment (selector 0x10)
    ; Flags: Present, DPL=0, Data (Writable)
    dw 0xFFFF        ; Limit 0:15
    dw 0x0000        ; Base 0:15
    db 0x00          ; Base 16:23
    db 0x93          ; P=1 DPL=0 S=1 Type=3 (Data: read/write)
    db 0xCF          ; G=1 B=1 (32-bit granular)
    db 0x00          ; Base 24:31
gdt_end:

gdt_descriptor:
    dw gdt_end - gdt_start - 1  ; Size (limit)
    dd gdt_start                ; Base address

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

msg_pm_start:
    db "Entering PM...", 0x0D, 0x0A, 0

; Boot signature
times 510 - ($ - $$) db 0
dw 0xAA55

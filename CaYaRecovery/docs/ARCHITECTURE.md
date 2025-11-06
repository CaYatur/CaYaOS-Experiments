# CaYaDev Recovery System - Proje Yapısı

## 📊 Sistem Mimarisi

```
┌─────────────────────────────────────────────────────────┐
│           CaYaDev Recovery System v1.0.0                │
│                                                         │
│  ┌─────────────────────────────────────────────────┐  │
│  │  MBR Boot Sector (512 byte)                    │  │
│  │  - CaYa Bootloader (Assembly)                  │  │
│  │  - Real Mode (16-bit)                          │  │
│  │  - Boot Menu Display                           │  │
│  │  - Kernel Loader                               │  │
│  └─────────────────────────────────────────────────┘  │
│                      ↓                                 │
│  ┌─────────────────────────────────────────────────┐  │
│  │  Protected Mode Entry                          │  │
│  │  - GDT Setup                                    │  │
│  │  - Protected Mode Activation                   │  │
│  └─────────────────────────────────────────────────┘  │
│                      ↓                                 │
│  ┌─────────────────────────────────────────────────┐  │
│  │  CaYa Kernel (0x10000)                         │  │
│  │  - Protected Mode (32-bit)                     │  │
│  │  - GDT, IDT Setup                              │  │
│  │  - Display "CaYaDev Recovery System"           │  │
│  │  - Paging & Memory Management                  │  │
│  └─────────────────────────────────────────────────┘  │
│                                                         │
└─────────────────────────────────────────────────────────┘

    ISO 9660 Bootable Image
    └── Boot.bin + Kernel.bin + Metadata
```

## 📁 Dizin Yapısı

```
CaYaRecovery/
│
├── src/                           Kaynak kod
│   ├── bootloader/
│   │   └── boot.asm              (512 byte MBR bootloader)
│   ├── kernel/
│   │   └── kernel.asm            (Protected Mode kernel)
│   └── boot_menu/
│       └── menu.asm              (Boot menu kodu)
│
├── build/                         Derlenmiş dosyalar
│   ├── boot.bin                  (Bootloader binary)
│   ├── kernel.bin                (Kernel binary)
│   └── CaYaRecovery.iso          (Bootable ISO image)
│
├── tools/                         Geliştirme araçları
│   └── (Araçlar buraya eklenecek)
│
├── docs/                          Dokümantasyon
│   ├── ARCHITECTURE.md            (Sistem mimarisi)
│   ├── BUILD.md                   (Derleme kılavuzu)
│   └── KERNEL_API.md              (Kernel API)
│
├── build.py                       Ana build script (Python 3)
└── Makefile                       Make ile derleme (Opsiyonel)
```

## 🔧 Teknik Özellikler

### Bootloader (boot.asm)
- **Başlangıç:** 0x7C00 (standart MBR konumu)
- **Mod:** Real Mode (16-bit x86)
- **Boyut:** Tam olarak 512 byte (MBR boyutu)
- **Boot Signature:** 0xAA55 (Magic number)
- **Görevler:**
  - Ekranı temizle
  - "CaYaDev Recovery System" mesajını göster
  - Kernel'i belleğe yükle
  - GDT hazırla
  - Protected Mode'a geç

### Kernel (kernel.asm)
- **Başlangıç:** 0x10000 (64 KB offset)
- **Mod:** Protected Mode (32-bit x86)
- **Özellikleri:**
  - GDT (Global Descriptor Table)
  - IDT (Interrupt Descriptor Table)
  - Paging yapılandırması
  - Video buffer'a yazma
  - "CaYaDev Recovery System - Kernel Loaded" mesajı

### ISO Yapısı
- **Format:** ISO 9660
- **Bootable:** El-Torito bootable CD
- **MBR:** boot.bin (512 byte)
- **Kernel:** kernel.bin
- **Endianness:** Little-endian (x86)

## 🔨 Derleme Gereksinimleri

### Gerekli Araçlar
1. **NASM** - Assembly derleyicisi
   ```bash
   # Linux (Debian/Ubuntu)
   sudo apt-get install nasm
   
   # macOS
   brew install nasm
   
   # Windows (WSL2)
   sudo apt-get install nasm
   ```

2. **Python 3.6+** - Build script için
   ```bash
   python3 build.py
   ```

### Opsiyonel Araçlar
- **xorriso** - Bootable ISO oluşturma (gelecek)
- **QEMU** - Emülasyon ve test
- **Make** - Makefile ile derleme (gelecek)

## 🚀 Derleme Adımları

### 1. Ortamı Hazırla

Windows'ta (PowerShell yönetici):
```powershell
wsl
sudo apt-get update
sudo apt-get install -y nasm
```

Linux'ta:
```bash
sudo apt-get install -y nasm
```

### 2. Projeyi Derle

```bash
cd c:\Users\cagan\Desktop\CaYaRecovery
python3 build.py
```

### 3. Sonuç

Başarılı olursa:
```
[SUCCESS] Bootloader derlemesi başarılı
[SUCCESS] Kernel derlemesi başarılı
[SUCCESS] ISO dosyası oluşturuldu
```

Üretilen dosyalar:
- `build/boot.bin` - Bootloader (512 byte)
- `build/kernel.bin` - Kernel
- `build/CaYaRecovery.iso` - Bootable ISO

## 🧪 Test Etme

### QEMU ile Test

```bash
# Sistem gereksinimleri
sudo apt-get install qemu-system-x86

# Test
qemu-system-i386 -cdrom build/CaYaRecovery.iso -m 256
```

Beklenen çıktı:
```
=========================================
   CaYaDev Recovery System v1.0.0
=========================================

CaYa Bootloader - Recovery System
Completely Custom Boot Solution

Press ENTER to continue...
```

Kernel yüklendikten sonra:
```
=========================================
CaYaDev Recovery System - Kernel Loaded
=========================================
```

### USB'ye Yazma

Linux'ta:
```bash
# USB sürücüyü bulun
lsblk

# ISO'yu yazın (sdX yerine gerçek cihazı yazın)
sudo dd if=build/CaYaRecovery.iso of=/dev/sdX bs=4M status=progress sync
```

Windows'ta:
- **Rufus** (https://rufus.ie/) veya
- **Ventoy** (https://www.ventoy.net/) kullanın

## 📚 Dosya Açıklamaları

### src/bootloader/boot.asm
**İçerik:**
- MBR başlığı
- BPB (BIOS Parameter Block)
- Boot menüsü
- Kernel yükleyici
- GDT tanımı
- Protected Mode girişi

**Anahtar Fonksiyonlar:**
- `clear_screen()` - Ekranı temizle
- `print_string()` - String yazdır
- `show_boot_menu()` - Boot menüsünü göster
- `load_kernel()` - Kernel'i yükle
- `enter_protected_mode()` - PM'ye geç

### src/kernel/kernel.asm
**İçerik:**
- Protected Mode entry point
- Video buffer yazma
- GDT tanımı
- IDT tanımı
- Paging tables
- Kernel stack

**Anahtar Fonksiyonlar:**
- `pm_clear_screen()` - Video buffer'ı temizle
- `pm_print_string()` - PM'de string yazdır

## 🔐 Boot Süreci

1. **BIOS POST** (Power-On Self Test)
2. **MBR Yüklemesi** (0x7C00 adresine)
3. **Boot.asm Çalıştırılması**
   - Ekran temizleme
   - Menü gösterme
4. **Kernel Yükleme**
5. **GDT Yapılandırması**
6. **Protected Mode Etkinleştirme**
7. **Kernel Çalıştırılması** (0x10000 adresinde)
8. **"CaYaDev Recovery System" Mesajı**

## 📊 Bellek Düzeni

```
0x0000 - 0x03FF     Interrupt Vector Table (IVT)
0x0400 - 0x04FF     BIOS Data Area
0x0500 - 0x7BFF     Kullanılabilir RAM
0x7C00 - 0x7DFF     MBR Boot Sector (boot.asm)
0x7E00 - 0xEFFFF    Yüklenen Program
0x10000 - 0xFFFFF   CaYa Kernel başlangıcı
0xA0000 - 0xAFFFF   Video RAM (64 KB)
0xB0000 - 0xB7FFF   Monochrome Display Buffer
0xB8000 - 0xBFFFF   Color Display Buffer (80x25 = 4000 byte)
0xE0000 - 0xEFFFF   BIOS ROM
0xF0000 - 0xFFFFF   BIOS ROM
```

## 🎯 Sonraki Adımlar

1. [ ] Disk I/O uygulaması
2. [ ] Interrupt handler'lar
3. [ ] Basit file sistem
4. [ ] Shell komutları
5. [ ] Recovery tools
6. [ ] UEFI desteği

## 📖 Kaynaklar

- **Intel 80386 Programmer's Reference Manual**
- **BIOS Interrupt Reference**
- **ISO 9660 Specification**
- **NASM Documentation** (https://www.nasm.us/)
- **OSDev.org** (https://wiki.osdev.org/)

## 📝 Not

Bu sistem tamamen **sıfırdan yazılmıştır**. Hiç bir üçüncü taraf bootloader veya kernel kütüphanesi kullanılmamıştır. Tüm kod **CaYaDev** tarafından yazılmıştır.

---

**Versiyon:** 1.0.0  
**Yazılan:** CaYaDev 2025  
**Durum:** Development

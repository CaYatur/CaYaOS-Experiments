# CaYaDev Recovery System - Derleme Kılavuzu

## 🚀 Hızlı Başlangıç

### Windows (PowerShell - Admin)

```powershell
# 1. WSL2 aç
wsl

# 2. Araçları yükle
sudo apt-get update
sudo apt-get install -y nasm python3

# 3. Proje dizinine gir
cd /mnt/c/Users/cagan/Desktop/CaYaRecovery

# 4. Derle
python3 build.py
```

### Linux

```bash
# 1. Araçları yükle
sudo apt-get update
sudo apt-get install -y nasm python3

# 2. Proje dizinine gir
cd ~/CaYaRecovery

# 3. Derle
python3 build.py
```

---

## 📦 Adım Adım: Gerekli Araçları Yükle

### NASM Kurulumu

**Linux (Debian/Ubuntu):**
```bash
sudo apt-get update
sudo apt-get install -y nasm
nasm -version
```

**macOS (Homebrew):**
```bash
brew install nasm
nasm -version
```

**Windows (WSL2):**
```bash
wsl
sudo apt-get install -y nasm
nasm -version
```

### Python 3 Kurulumu

Linux/WSL2'de zaten kurulu olabilir, kontrol edin:
```bash
python3 --version
```

Yoksa:
```bash
sudo apt-get install -y python3
```

---

## 🔨 Derleme Süreci

### Adım 1: Proje Dizinine Gir

```bash
# Linux/macOS
cd ~/CaYaRecovery

# Windows (WSL2)
cd /mnt/c/Users/cagan/Desktop/CaYaRecovery
```

### Adım 2: Build Script'i Çalıştır

```bash
python3 build.py
```

### Adım 3: Çıktı Dosyaları

Başarılı derlemenin sonucu:

```
build/
├── boot.bin          (512 byte - MBR Bootloader)
├── kernel.bin        (Kernel binary)
└── CaYaRecovery.iso  (Bootable ISO image)
```

---

## ✅ Derleme Sonuçları

### Başarılı Derleme Mesajları

```
[SUCCESS] NASM bulundu: 2.15.05
[SUCCESS] Bootloader derlemesi başarılı: build/boot.bin (512 byte)
[SUCCESS] Kernel derlemesi başarılı: build/kernel.bin (X byte)
[SUCCESS] ISO dosyası oluşturuldu: build/CaYaRecovery.iso

Bootloader: build/boot.bin
Kernel:     build/kernel.bin
ISO:        build/CaYaRecovery.iso
```

### Dosya Boyutları

| Dosya | Boyut | Not |
|-------|-------|-----|
| boot.bin | 512 byte | Tam olarak 512 olmalı (MBR) |
| kernel.bin | ~2-10 KB | Değişken |
| CaYaRecovery.iso | ~20-50 KB | Minimal sistem |

---

## 🧪 Test Etme

### QEMU ile Test

**1. QEMU Kurulumu:**

```bash
sudo apt-get install qemu-system-x86 qemu-utils
```

**2. Boot Testi:**

```bash
qemu-system-i386 -cdrom build/CaYaRecovery.iso -m 256 -display gtk
```

Alternatif display seçenekleri:
- `-display gtk` - GUI window
- `-display sdl` - SDL window
- `-nographic` - Text mode (Ctrl+A X ile çık)

**3. Beklenen Çıktı:**

Boot menüsü görülmelidir:
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

---

## 💾 USB'ye Yazma

### Linux'ta

```bash
# 1. USB sürücüyü bulun
lsblk

# Örnek çıktı:
# sda      8:0    0 238.47G  0 disk
# sdb      8:16   1   14.6G  0 disk  <- USB (sdX)

# 2. ISO'yu yazın (sdX yerine gerçek cihazı yazın)
sudo dd if=build/CaYaRecovery.iso of=/dev/sdb bs=4M status=progress sync

# 3. Tamamlandığında
sync
```

### Windows'ta

**Yöntem 1: Rufus**

1. https://rufus.ie/ adresinden indir
2. Rufus'u çalıştır
3. USB sürücü seç
4. `build/CaYaRecovery.iso` seç
5. **START** butonuna bas
6. Uyarı mesajlarını kabul et

**Yöntem 2: Ventoy**

1. https://www.ventoy.net/ adresinden indir
2. Ventoy'u USB'ye yükle
3. `build/CaYaRecovery.iso` dosyasını USB'nin kök dizinine kopyala
4. USB'den boot et

---

## ⚠️ Sorun Giderme

### "NASM bulunamadı"

```bash
# Kurulum
sudo apt-get install nasm

# Kontrol
nasm -version
```

### "Permission denied" hatası (USB yazarken)

```bash
# Doğru cihazı kullandığınız emin olun
lsblk

# Sudo ile çalıştırın
sudo dd if=build/CaYaRecovery.iso of=/dev/sdb bs=4M status=progress
```

### Derleme hatası: "Unexpected token"

Boot.asm veya kernel.asm dosyasında yazım hatası olabilir.

Kontrol edin:
- Satır sonları (CRLF vs LF)
- Boşluk karakterleri (tab vs space)
- Assembly syntax

### USB boot etmiyor

1. **BIOS ayarlarını kontrol edin:**
   - USB boot'u etkinleştir
   - Boot sırasını ayarla

2. **Farklı USB port deneyin**

3. **ISO'yu yeniden yazın:**
   ```bash
   sudo dd if=build/CaYaRecovery.iso of=/dev/sdb bs=4M status=progress sync
   ```

4. **Ventoy kullanmayı deneyin**

---

## 🔧 Manuel Derleme (Adım Adım)

NASM'i doğrudan çalıştırmak istiyorsanız:

### Bootloader Derleme

```bash
nasm -f bin -o build/boot.bin src/bootloader/boot.asm
```

### Kernel Derleme

```bash
nasm -f bin -o build/kernel.bin src/kernel/kernel.asm
```

### ISO Oluşturma (xorriso kullanarak)

```bash
# xorriso kurulumu
sudo apt-get install xorriso

# ISO oluşturma
xorriso -as mkisofs \
  -o build/CaYaRecovery.iso \
  -b build/boot.bin \
  -no-emul-boot \
  build/
```

---

## 📊 Derleme Çıktıları Analizi

### boot.bin Kontrol Etme

```bash
# Dosya bilgisi
file build/boot.bin

# Boyutu doğrula
ls -l build/boot.bin
# 512 bayt olmalı

# İçeriği görüntüle (hex)
hexdump -C build/boot.bin | head -20

# Boot signature kontrol
hexdump -C build/boot.bin | tail -1
# "0001f0 00 00 00 00 00 00 00 00 00 00 55 aa" olmalı (0xAA55)
```

### kernel.bin Kontrol Etme

```bash
# Dosya bilgisi
file build/kernel.bin

# Boyutu
ls -lh build/kernel.bin

# İçeriği görüntüle
hexdump -C build/kernel.bin | head -10
```

---

## 🎯 İleri Derleme Seçenekleri

### Makefile Kullanarak (Gelecek)

```makefile
# Makefile örneği

.PHONY: all clean

NASM := nasm
PYTHON := python3

all: build

build:
	$(PYTHON) build.py

clean:
	rm -f build/*.bin build/*.iso

test:
	qemu-system-i386 -cdrom build/CaYaRecovery.iso

usb:
	@echo "USB'ye yazmak için:"
	@echo "sudo dd if=build/CaYaRecovery.iso of=/dev/sdX bs=4M"
```

### CMake Kullanarak (Gelecek)

```cmake
# CMakeLists.txt örneği

cmake_minimum_required(VERSION 3.10)
project(CaYaRecovery)

find_program(NASM nasm REQUIRED)

add_custom_command(OUTPUT boot.bin
    COMMAND ${NASM} -f bin -o boot.bin boot.asm
    DEPENDS boot.asm
)

add_custom_target(build ALL DEPENDS boot.bin)
```

---

## 📝 Derleme Logları

Derleme sırasında detaylı loglar görmek istiyorsanız:

```bash
# Verbose mode
python3 -v build.py 2>&1 | tee build.log

# NASM verbose
nasm -f bin -o build/boot.bin src/bootloader/boot.asm -L
```

---

## 🎓 Öğrenme Kaynakları

### Assembly Yazım

- **NASM Tutorial:** https://www.nasm.us/
- **x86 Assembly:** https://en.wikibooks.org/wiki/X86_Assembly
- **OSDev Assembly:** https://wiki.osdev.org/Assembly

### Bootloader Geliştirme

- **MBR Yazma:** https://wiki.osdev.org/MBR_(x86)
- **Boot Protocol:** https://wiki.osdev.org/Boot_protocol
- **Real Mode:** https://wiki.osdev.org/Real_Mode

### Kernel Geliştirme

- **Protected Mode:** https://wiki.osdev.org/Protected_Mode
- **GDT:** https://wiki.osdev.org/Global_Descriptor_Table
- **IDT:** https://wiki.osdev.org/Interrupt_Descriptor_Table
- **Paging:** https://wiki.osdev.org/Paging

---

## ✨ En İyi Uygulamalar

1. **Dosya Yapısını Koruyun**
   - src/ altında kaynak kodlar
   - build/ altında çıktı dosyaları
   - docs/ altında dokümantasyon

2. **Assembly Kodunu Açıkla**
   - Her fonksiyona yorum ekle
   - Karmaşık parçaları açıkla

3. **Test Sık Yapın**
   - Her değişiklikten sonra derle
   - QEMU ile boot sırasında kontrol et

4. **Git Kullanın**
   ```bash
   git init
   git add .
   git commit -m "Initial commit"
   ```

---

## 📞 Yardım

Sorun mu var?

1. **Yukarıdaki sorun giderme bölümünü kontrol et**
2. **Dosya yapısını doğrula:**
   ```bash
   ls -la src/bootloader/
   ls -la src/kernel/
   ```
3. **Build loglarını kontrol et**
4. **OSDev.org wiki'sini oku**

---

**Versiyon:** 1.0.0  
**Tarih:** 2025-11-06  
**Durum:** Production Ready ✓

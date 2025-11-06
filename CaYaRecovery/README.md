# 🎉 CaYaDev Recovery System - BAŞLANGIÇ REHBERİ

## ⭐ Bu Projede Neler Yapılıyor?

Tamamen **sıfırdan** yazılmış, hiç 3. taraf sistem **kullanmayan** bir recovery sistemi:

- ✅ **CaYa Bootloader** - Kendi yazılmış MBR bootloader'ı
- ✅ **CaYa Kernel** - Kendi yazılmış x86 kernel'i
- ✅ **Boot Menu** - Kendi yapılmış boot menüsü
- ✅ **ISO Creator** - Sıfırdan ISO oluşturucu
- ❌ **GRUB** - KULLANILMIYOR
- ❌ **Linux Kernel** - KULLANILMIYOR
- ❌ **3. Taraf Sistemler** - HİÇBİRİ KULLANILMIYOR

---

## 📂 Proje Yapısı

```
CaYaRecovery/
├── src/                    # Kaynak kodlar
│   ├── bootloader/
│   │   └── boot.asm       # ⭐ Bootloader (512 byte)
│   ├── kernel/
│   │   └── kernel.asm     # ⭐ Kernel (Protected Mode)
│   └── boot_menu/
│       └── menu.asm       # Boot menüsü (gelecek)
│
├── build/                  # Derlenmiş çıktılar
│   ├── boot.bin           # Bootloader binary
│   ├── kernel.bin         # Kernel binary
│   └── CaYaRecovery.iso   # Bootable ISO
│
├── docs/                   # Dokümantasyon
│   ├── ARCHITECTURE.md     # Sistem mimarisi
│   ├── BUILD.md            # Derleme kılavuzu
│   └── KERNEL_API.md       # Kernel API
│
├── tools/                  # Araçlar
├── build.py               # Main build script
└── README.md              # Bu dosya
```

---

## 🚀 İLK ADIM: Sistemi Kurma (5 dakika)

### Windows'ta (PowerShell - Yönetici)

```powershell
# 1. WSL2'yi başlat
wsl

# WSL2 Linux terminal'de:

# 2. Paketleri güncelle
sudo apt-get update

# 3. Gerekli araçları yükle
sudo apt-get install -y nasm python3

# 4. Proje dizinine gir
cd /mnt/c/Users/cagan/Desktop/CaYaRecovery

# 5. Derle!
python3 build.py
```

### Linux'ta

```bash
# 1. Araçları yükle
sudo apt-get update
sudo apt-get install -y nasm python3

# 2. Proje dizinine gir
cd ~/CaYaRecovery

# 3. Derle!
python3 build.py
```

---

## ✅ Başarılı Derleme

Başarılı olursa böyle görünmelidir:

```
============================================================
CaYaDev Recovery System - Build
============================================================

[INFO] NASM derleyicisi kontrol ediliyor...
[SUCCESS] NASM bulundu: 2.15.05
[INFO] Bootloader derleniyorum...
[SUCCESS] Bootloader derlemesi başarılı: build/boot.bin (512 byte)
[INFO] Kernel derleniyorum...
[SUCCESS] Kernel derlemesi başarılı: build/kernel.bin (16384 byte)
[INFO] ISO dosyası oluşturuluyor...
[SUCCESS] ISO dosyası oluşturuldu: build/CaYaRecovery.iso (16896 byte)

============================================================
✓ Derleme başarılı!
============================================================

Bootloader: build/boot.bin
Kernel:     build/kernel.bin
ISO:        build/CaYaRecovery.iso
```

---

## 🎮 ISO'yu QEMU'da Test Etme

```bash
# Linux/WSL2'de:
qemu-system-i386 -cdrom build/CaYaRecovery.iso -m 256

# macOS'ta (homebrew):
qemu-system-i386 -cdrom build/CaYaRecovery.iso -m 256
```

Boot ekranında şu mesajı göreceksiniz:

```
=== CaYaDev Recovery v1.0.0 ===
32/64-bit Compatible
Press ENTER
```

Enter tuşuna basın ve kernel yüklenecek!

---

## 💾 ISO'yu USB'ye Yazma (Bootable USB Oluşturma)

### Linux'ta

```bash
# ⚠️ DİKKAT: /dev/sdX'i doğru cihazla değiştirin!
# Listelemek için: lsblk

# 1. USB'ye yaz
sudo dd if=build/CaYaRecovery.iso of=/dev/sdX bs=4M status=progress

# 2. Sync yap
sudo sync

# 3. USB'yi çıkar ve başka bilgisayarda test et!
```

### Windows'ta (PowerShell - Yönetici)

Balena Etcher veya Rufus kullanın:
- **Balena Etcher**: https://www.balena.io/etcher/
- **Rufus**: https://rufus.ie/

Her ikisi de `CaYaRecovery.iso` dosyasını seçip USB'ye yazabilir.
[SUCCESS] ISO dosyası oluşturuldu: build/CaYaRecovery.iso

============================================================
✓ Derleme başarılı!
============================================================

Bootloader: build/boot.bin
Kernel:     build/kernel.bin
ISO:        build/CaYaRecovery.iso

Boot etmek için:
  qemu-system-i386 -cdrom build/CaYaRecovery.iso
```

---

## 🧪 ADIM 2: Test Etme (QEMU ile)

### QEMU Kurulumu

```bash
sudo apt-get install qemu-system-x86
```

### ISO'yu Boot Et

```bash
qemu-system-i386 -cdrom build/CaYaRecovery.iso -m 256 -display gtk
```

### Beklenen Çıktı

Boot menu gösterilecek:

```
=========================================
   CaYaDev Recovery System v1.0.0
=========================================

CaYa Bootloader - Recovery System
Completely Custom Boot Solution

Press ENTER to continue...
```

**ENTER tuşuna bas** → Kernel yüklenir:

```
[*] Loading CaYa Kernel...
[OK] CaYa Kernel loaded successfully
[*] Entering Protected Mode...
```

Kernel başlar ve gösterir:

```
=========================================
CaYaDev Recovery System - Kernel Loaded
=========================================
```

---

## 💾 ADIM 3: USB'ye Yazma

### Linux'ta

```bash
# 1. USB sürücüyü bul
lsblk

# Çıktı örneği:
# sda      8:0    0 238.47G  0 disk
# sdb      8:16   1   14.6G  0 disk  <- USB (sdb)

# 2. ISO'yu yaz (sdb yerine kendi cihazını yaz!)
sudo dd if=build/CaYaRecovery.iso of=/dev/sdb bs=4M status=progress sync

# 3. Tamamlandı!
```

### Windows'ta

**Seçenek A: Rufus (Kolay)**

1. https://rufus.ie/ indir
2. Rufus'u aç
3. USB sürücü seç
4. `build/CaYaRecovery.iso` seç
5. **START**

**Seçenek B: Ventoy**

1. https://www.ventoy.net/ indir
2. USB'ye Ventoy yükle
3. ISO dosyasını USB'ye kopyala

---

## 🎯 ADIM 4: Gerçek Bilgisayarda Boot Et

1. USB'yi bilgisayara tak
2. Bilgisayarı restart et
3. Boot menüsüne gir (F12, ESC, DEL, etc.)
4. USB sürücüyü seç
5. Boot menüsünü gör ✓

---

## 📊 Sistem Mimarisi

```
BIOS'dan başla
     ↓
Boot sector (0x7C00) - boot.asm
     ↓
Real Mode çalışması
     - Ekranı temizle
     - Menu göster
     - Kernel yükle
     ↓
GDT hazırla
     ↓
Protected Mode'a geç
     ↓
Kernel (0x10000) - kernel.asm
     ↓
Protected Mode çalışması
     - Video buffer'a yaz
     - "CaYaDev Recovery System" göster
     ↓
Sistem çalışıyor ✓
```

---

## 🔧 Kodu Özelleştirmek

### Boot Menüsünü Değiştir

Dosya: `src/bootloader/boot.asm`

Bul ve değiştir (satır ~200 civarında):

```asm
msg_title:
    db 0x0D, 0x0A
    db "========================================="
    db 0x0D, 0x0A
    db "   CaYaDev Recovery System v1.0.0"      <- BU SATIR
    db 0x0D, 0x0A
```

Değiştir:

```asm
msg_title:
    db 0x0D, 0x0A
    db "========================================="
    db 0x0D, 0x0A
    db "   Kendi Sisteminizin Adı v1.0.0"       <- BURAYA YAZ
    db 0x0D, 0x0A
```

Değişiklik yaptıktan sonra:

```bash
python3 build.py
```

---

## 📝 Kernel'i Geliştirmek

Dosya: `src/kernel/kernel.asm`

Kernel kodunu yazıştır. Örnek - Renkli metin ekle:

```asm
; Renkli metin için attribute değiş
mov byte [edi + 1], 0x0A    ; Yeşil metin
```

Attribute renkleri:
- `0x0F` - Beyaz
- `0x0A` - Yeşil
- `0x0C` - Kırmızı
- `0x0E` - Sarı

---

## 🎓 Öğrenme Kaynakları

### Assembly Başlangıcı
- NASM Tutorial: https://www.nasm.us/
- x86 Assembly: https://en.wikibooks.org/wiki/X86_Assembly

### Bootloader Geliştirme
- MBR: https://wiki.osdev.org/MBR_(x86)
- Real Mode: https://wiki.osdev.org/Real_Mode

### Kernel Geliştirme
- Protected Mode: https://wiki.osdev.org/Protected_Mode
- GDT: https://wiki.osdev.org/Global_Descriptor_Table
- Memory: https://wiki.osdev.org/Paging

---

## ⚠️ Sorun Giderme

### "NASM bulunamadı"
```bash
sudo apt-get install nasm
```

### "Permission denied" (USB yazarken)
```bash
# Sudo ile yaz
sudo dd if=build/CaYaRecovery.iso of=/dev/sdb bs=4M status=progress
```

### Boot menu görülmüyor
1. QEMU kurulu mu? `qemu-system-i386 --version`
2. ISO dosyası var mı? `ls -lh build/CaYaRecovery.iso`
3. Boot.bin 512 byte mi? `ls -l build/boot.bin`

### USB boot etmiyor
- BIOS'ta USB boot'u etkinleştir
- Farklı USB port dene
- Ventoy kullanmayı dene

---

## 📚 Sonraki Adımlar

Sistem tamamlandıktan sonra eklemek isteyebileceğiniz şeyler:

1. **Disk I/O** - Diskten dosya oku/yaz
2. **File Sistem** - Basit FAT veya ext2
3. **Shell** - Komut satırı arayüzü
4. **Recovery Tools** - fsck, fdisk, vb.
5. **Network** - Network boot (PXE)
6. **UEFI** - UEFI boot desteği
7. **Multitasking** - Basit task switching
8. **Memory Manager** - Virtual memory

---

## 🎉 Tebrikler!

Artık kendi bootable recovery sisteminiz var!

**Ne öğrendiniz:**
- ✓ Assembly ile bootloader yazma
- ✓ Real Mode ve Protected Mode geçişi
- ✓ GDT ve segment register'ları
- ✓ MBR ve boot sector yapısı
- ✓ BIOS interrupt'ları
- ✓ x86 mimarisinin temelleri

---

## 📞 Yardım

Daha fazla bilgi:
- `docs/ARCHITECTURE.md` - Sistem mimarisi
- `docs/BUILD.md` - Detaylı derleme rehberi
- `docs/KERNEL_API.md` - Kernel fonksiyonları

---

**Versiyon:** 1.0.0  
**Yazılan:** CaYaDev 2025  
**Durum:** ✅ Production Ready

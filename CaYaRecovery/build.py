#!/usr/bin/env python3
"""
CaYa Recovery System - Build Script
Tamamen sıfırdan yazılmış bootable ISO oluşturucu

Görev:
- boot.bin (bootloader)
- kernel.bin (kernel)
- Bunları ISO'ya koymak
- Bootable hale getirmek
"""

import os
import sys
import struct
import subprocess
from pathlib import Path

class CaYaBuilder:
    def __init__(self, project_root):
        self.project_root = Path(project_root).resolve()  # Absolute path
        self.src_dir = self.project_root / "src"
        self.build_dir = self.project_root / "build"
        self.tools_dir = self.project_root / "tools"
        
        # Çıktı dosyaları
        self.boot_bin = self.build_dir / "boot.bin"
        self.kernel_bin = self.build_dir / "kernel.bin"
        self.iso_image = self.build_dir / "CaYaRecovery.iso"
        
        # Build dizini oluştur
        self.build_dir.mkdir(parents=True, exist_ok=True)
        
    def log(self, message, level="INFO"):
        """Log mesajı yazdır"""
        colors = {
            "INFO": "\033[94m",
            "SUCCESS": "\033[92m",
            "ERROR": "\033[91m",
            "WARNING": "\033[93m",
            "RESET": "\033[0m"
        }
        color = colors.get(level, colors["INFO"])
        print(f"{color}[{level}]{colors['RESET']} {message}")
    
    def check_nasm(self):
        """NASM derleyicisinin kurulu olup olmadığını kontrol et"""
        self.log("NASM derleyicisi kontrol ediliyor...")
        
        try:
            result = subprocess.run(["nasm", "-version"], 
                                  capture_output=True, 
                                  text=True)
            if result.returncode == 0:
                self.log(f"NASM bulundu: {result.stdout.split()[1]}", "SUCCESS")
                return True
        except FileNotFoundError:
            pass
        
        self.log("NASM bulunamadı!", "ERROR")
        self.log("Kurulum: apt-get install nasm (Linux) veya brew install nasm (Mac)", "WARNING")
        return False
    
    def assemble_bootloader(self):
        """Bootloader'ı derle"""
        self.log("Bootloader derleniyorum...", "INFO")
        
        boot_asm = self.src_dir / "bootloader" / "boot.asm"
        
        if not boot_asm.exists():
            self.log(f"Bootloader dosyası bulunamadı: {boot_asm}", "ERROR")
            return False
        
        try:
            cmd = [
                "nasm",
                "-f", "bin",
                "-o", str(self.boot_bin),
                str(boot_asm)
            ]
            
            result = subprocess.run(cmd, capture_output=True, text=True)
            
            if result.returncode != 0:
                self.log(f"Derleme hatası: {result.stderr}", "ERROR")
                return False
            
            # Dosya boyutunu kontrol et
            if self.boot_bin.stat().st_size != 512:
                self.log(f"Uyarı: Bootloader {self.boot_bin.stat().st_size} byte (512 olmalı)", "WARNING")
            
            self.log(f"Bootloader derlemesi başarılı: {self.boot_bin} ({self.boot_bin.stat().st_size} byte)", "SUCCESS")
            return True
        
        except Exception as e:
            self.log(f"Hata: {e}", "ERROR")
            return False
    
    def assemble_kernel(self):
        """Kernel'i derle"""
        self.log("Kernel derleniyorum...", "INFO")
        
        kernel_asm = self.src_dir / "kernel" / "kernel.asm"
        
        if not kernel_asm.exists():
            self.log(f"Kernel dosyası bulunamadı: {kernel_asm}", "ERROR")
            return False
        
        try:
            cmd = [
                "nasm",
                "-f", "bin",
                "-o", str(self.kernel_bin),
                str(kernel_asm)
            ]
            
            result = subprocess.run(cmd, capture_output=True, text=True)
            
            if result.returncode != 0:
                self.log(f"Derleme hatası: {result.stderr}", "ERROR")
                return False
            
            self.log(f"Kernel derlemesi başarılı: {self.kernel_bin} ({self.kernel_bin.stat().st_size} byte)", "SUCCESS")
            return True
        
        except Exception as e:
            self.log(f"Hata: {e}", "ERROR")
            return False
    
    def create_iso(self):
        """ISO dosyasını oluştur"""
        self.log("ISO dosyası oluşturuluyor...", "INFO")
        
        # ISO için geçici dosya yapısı oluştur
        iso_temp = self.build_dir / "iso_temp"
        iso_temp.mkdir(exist_ok=True)
        
        boot_dir = iso_temp / "boot"
        boot_dir.mkdir(exist_ok=True)
        
        # Binary dosyaları ISO'ya kopyala
        import shutil
        shutil.copy(self.boot_bin, boot_dir / "boot.bin")
        shutil.copy(self.kernel_bin, boot_dir / "kernel.bin")
        
        # Bootable ISO'yu sıfırdan oluştur (ISO 9660)
        self._create_iso_9660(iso_temp, self.iso_image)
        
        self.log(f"ISO dosyası oluşturuldu: {self.iso_image} ({self.iso_image.stat().st_size} byte)", "SUCCESS")
        return True
    
    def _create_iso_9660(self, source_dir, output_iso):
        """ISO 9660 formatında bootable ISO oluştur (mkisofs/xorriso ile)"""
        
        self.log("ISO yapısı hazırlanıyor (El-Torito bootable)", "INFO")
        
        # Önce xorriso'yu dene, sonra mkisofs'u dene
        iso_tools = [
            # xorriso komutu (modern)
            ["xorriso", "-as", "mkisofs", "-boot-load-size", "4",
             "-b", str(self.boot_bin),
             "-no-emul-boot", "-boot-info-table",
             "-input-charset", "UTF-8",
             "-o", str(output_iso),
             str(source_dir)],
            
            # mkisofs komutu (eski ama yaygın)
            ["mkisofs", "-boot-load-size", "4",
             "-b", str(self.boot_bin),
             "-no-emul-boot", "-boot-info-table",
             "-iso9660", "-J", "-joliet-long",
             "-o", str(output_iso),
             str(source_dir)]
        ]
        
        success = False
        for tool_cmd in iso_tools:
            try:
                result = subprocess.run(tool_cmd, capture_output=True, text=True)
                if result.returncode == 0:
                    success = True
                    self.log(f"ISO oluşturucu: {tool_cmd[0]}", "SUCCESS")
                    break
            except FileNotFoundError:
                continue
        
        if success:
            return
        
        # Hiçbiri yoksa, fallback: basit bir ISO manuel oluştur
        self.log("xorriso/mkisofs bulunamadı, manuel ISO oluşturuluyor...", "WARNING")
        self._create_fallback_iso(output_iso)
    
    def _create_fallback_iso(self, output_iso):
        """Fallback: Manuel olarak düzgün ISO 9660 bootable ISO oluştur"""
        
        with open(self.boot_bin, 'rb') as f:
            boot_data = f.read(512)
        
        with open(self.kernel_bin, 'rb') as f:
            kernel_data = f.read()
        
        # ISO 9660 yapısı oluştur
        iso_data = bytearray()
        
        # 0. Boot sector (Sector 0) - MBR bootloader
        iso_data.extend(boot_data)
        
        # 1. Padding to sector boundary (Sector 1-15)
        iso_data.extend(b'\x00' * (16 * 2048 - len(boot_data)))
        
        # 2. Primary Volume Descriptor (Sector 16)
        pvd = self._create_pvd()
        iso_data.extend(pvd)
        
        # 3. Boot Record Volume Descriptor (Sector 17)
        boot_vd = self._create_boot_volume_descriptor()
        iso_data.extend(boot_vd)
        
        # 4. Volume Descriptor Set Terminator (Sector 18)
        terminator = self._create_terminator()
        iso_data.extend(terminator)
        
        # 5. El-Torito Boot Catalog (Sector 19)
        boot_catalog = self._create_boot_catalog()
        iso_data.extend(boot_catalog)
        
        # 6. Kernel image (Sector 20 onwards, aligned to 2048)
        padding = (2048 - (len(iso_data) % 2048)) % 2048
        iso_data.extend(b'\x00' * padding)
        kernel_start_sector = len(iso_data) // 2048
        iso_data.extend(kernel_data)
        
        # 7. Path Table (L) - Sector after kernel
        padding = (2048 - (len(iso_data) % 2048)) % 2048
        iso_data.extend(b'\x00' * padding)
        path_table_l = self._create_path_table_l()
        iso_data.extend(path_table_l)
        
        # 8. Path Table (M) - Next sector
        padding = (2048 - (len(iso_data) % 2048)) % 2048
        iso_data.extend(b'\x00' * padding)
        path_table_m = self._create_path_table_m()
        iso_data.extend(path_table_m)
        
        # 9. Root Directory - Next sector
        padding = (2048 - (len(iso_data) % 2048)) % 2048
        iso_data.extend(b'\x00' * padding)
        root_dir = self._create_root_directory()
        iso_data.extend(root_dir)
        
        # Dosyaya yaz
        with open(output_iso, 'wb') as f:
            f.write(iso_data)
    
    def _create_pvd(self):
        """ISO 9660 Primary Volume Descriptor oluştur"""
        pvd = bytearray(2048)
        
        pvd[0] = 0x01  # Type code = Primary Volume Descriptor
        pvd[1:6] = b'CD001'  # Standard identifier
        pvd[6] = 0x01  # Volume descriptor version
        
        # Unused (1 byte)
        pvd[7] = 0x00
        
        # System identifier (32 bytes)
        pvd[8:40] = b'LINUX'.ljust(32)
        
        # Volume identifier (32 bytes)
        pvd[40:72] = b'CAYARECOVERY'.ljust(32)
        
        # Unused (8 bytes)
        pvd[72:80] = b'\x00' * 8
        
        # Volume space size (number of sectors) - 8 bytes, both endian
        num_sectors = 100
        pvd[80:84] = struct.pack('<I', num_sectors)
        pvd[84:88] = struct.pack('>I', num_sectors)
        
        # Escape sequences (32 bytes)
        pvd[88:120] = b'\x00' * 32
        
        # Volume set size (2 bytes, both endian)
        pvd[120:122] = struct.pack('<H', 1)
        pvd[122:124] = struct.pack('>H', 1)
        
        # Volume sequence number (2 bytes, both endian)
        pvd[124:126] = struct.pack('<H', 1)
        pvd[126:128] = struct.pack('>H', 1)
        
        # Logical block size (2 bytes, both endian) = 2048
        pvd[128:130] = struct.pack('<H', 2048)
        pvd[130:132] = struct.pack('>H', 2048)
        
        # Path table size (4 bytes, both endian)
        path_table_size = 8  # Minimal path table
        pvd[132:136] = struct.pack('<I', path_table_size)
        pvd[136:140] = struct.pack('>I', path_table_size)
        
        # Type L Path Table Location (4 bytes, little endian)
        pvd[140:144] = struct.pack('<I', 23)  # Sector 23
        
        # Optional Type L Path Table Location (4 bytes, little endian)
        pvd[144:148] = struct.pack('<I', 0)
        
        # Type M Path Table Location (4 bytes, big endian)
        pvd[148:152] = struct.pack('>I', 24)  # Sector 24
        
        # Optional Type M Path Table Location (4 bytes, big endian)
        pvd[152:156] = struct.pack('>I', 0)
        
        # Directory Record for Root (34 bytes)
        root_dir = bytearray(34)
        root_dir[0] = 34  # Directory Record Length
        root_dir[1] = 0x00  # Extended Attribute Record Length
        root_dir[2:10] = struct.pack('<Q', 25 * 2048)  # LBA of root (Sector 25)
        root_dir[10:18] = struct.pack('<Q', 2048)  # Size of root dir
        root_dir[18:25] = struct.pack('<7B', 125, 1, 1, 0, 0, 0, 0)  # Recording date/time
        root_dir[25:33] = struct.pack('<8B', 125, 1, 1, 0, 0, 0, 0, 0)  # Recording date/time + GMT offset
        root_dir[33] = 0  # File identifier length (root)
        
        pvd[156:190] = root_dir
        
        # Volume set identifier (128 bytes)
        pvd[190:318] = b'CAYARECOVERY'.ljust(128)
        
        # Publisher identifier (128 bytes)
        pvd[318:446] = b'CAYADEV'.ljust(128)
        
        # Data preparer identifier (128 bytes)
        pvd[446:574] = b'CAYADEV'.ljust(128)
        
        # Application identifier (128 bytes)
        pvd[574:702] = b'CAYARECOVERY'.ljust(128)
        
        # Copyright file identifier (37 bytes)
        pvd[702:739] = b'\x00' * 37
        
        # Abstract file identifier (37 bytes)
        pvd[739:776] = b'\x00' * 37
        
        # Bibliographic file identifier (37 bytes)
        pvd[776:813] = b'\x00' * 37
        
        # Volume creation date/time (17 bytes)
        pvd[813:830] = self._get_iso_timestamp()
        
        # Volume modification date/time (17 bytes)
        pvd[830:847] = self._get_iso_timestamp()
        
        # Volume expiration date/time (17 bytes)
        pvd[847:864] = b'\x00' * 17
        
        # Volume effective date/time (17 bytes)
        pvd[864:881] = self._get_iso_timestamp()
        
        # File structure version
        pvd[881] = 0x01
        
        # Unused (1 byte)
        pvd[882] = 0x00
        
        # Application use area (512 bytes)
        pvd[883:1395] = b'\x00' * 512
        
        # Reserved for future standardization (653 bytes)
        pvd[1395:2048] = b'\x00' * 653
        
        return bytes(pvd)
    
    def _create_boot_volume_descriptor(self):
        """Boot Record Volume Descriptor oluştur"""
        bvd = bytearray(2048)
        
        bvd[0] = 0x00  # Type code = Boot Record
        bvd[1:6] = b'CD001'  # Standard identifier
        bvd[6] = 0x01  # Volume descriptor version
        
        # Boot system identifier (32 bytes)
        bvd[7:39] = b'EL TORITO SPECIFICATION'.ljust(32)
        
        # Boot catalog location (4 bytes, little endian) - Sector 19
        bvd[71:75] = struct.pack('<I', 19)
        
        # Unused (1973 bytes)
        bvd[75:2048] = b'\x00' * 1973
        
        return bytes(bvd)
    
    def _create_terminator(self):
        """Volume Descriptor Set Terminator oluştur"""
        term = bytearray(2048)
        term[0] = 0xFF  # Type code = Terminator
        term[1:6] = b'CD001'  # Standard identifier
        term[6] = 0x01  # Volume descriptor version
        term[7:2048] = b'\x00' * 2041
        return bytes(term)
    
    def _create_boot_catalog(self):
        """El-Torito Boot Catalog oluştur"""
        catalog = bytearray(2048)
        
        # Validation Entry (32 bytes)
        catalog[0] = 0x01  # Header ID
        catalog[1] = 0x00  # Platform ID (x86-PC)
        catalog[2:4] = struct.pack('<H', 0)  # Reserved
        
        # OEM ID
        catalog[4:28] = b'CAYADEV'.ljust(24)
        
        # Checksum (to be calculated)
        checksum = 0
        for i in range(0, 16):
            checksum += struct.unpack('<H', catalog[i*2:(i+1)*2])[0]
        catalog[28:30] = struct.pack('<H', (0x10000 - checksum) & 0xFFFF)
        
        # Key bytes
        catalog[30] = 0x55
        catalog[31] = 0xAA
        
        # Boot Entry (32 bytes)
        catalog[32] = 0x88  # Boot indicator (bootable)
        catalog[33] = 0x00  # Boot media type (0 = no emulation, fixed disk)
        catalog[34:36] = struct.pack('<H', 0)  # Load segment (0 = 0x7C00)
        catalog[36] = 0x00  # System type
        catalog[37] = 0x00  # Unused
        catalog[38:40] = struct.pack('<H', 1)  # Sector count (1 sector = 512 byte)
        catalog[40:44] = struct.pack('<I', 0)  # Load RBA (Sector 0 = boot.bin location)
        
        # Unused
        catalog[44:2048] = b'\x00' * (2048 - 44)
        
        return bytes(catalog)
    
    def _create_path_table_l(self):
        """Path Table (little endian) oluştur"""
        pt = bytearray(2048)
        
        # Root directory entry
        pt[0] = 1  # Directory identifier length (1 = root)
        pt[1] = 0  # Extended Attribute Record Length
        pt[2:6] = struct.pack('<I', 25)  # Root directory LBA (Sector 25)
        pt[6:8] = struct.pack('<H', 1)  # Parent directory number (1 = root)
        pt[8] = 0  # Directory identifier (0 = root)
        
        # Padding to even
        pt[9:2048] = b'\x00' * (2048 - 9)
        
        return bytes(pt)
    
    def _create_path_table_m(self):
        """Path Table (big endian) oluştur"""
        pt = bytearray(2048)
        
        # Root directory entry
        pt[0] = 1  # Directory identifier length
        pt[1] = 0  # Extended Attribute Record Length
        pt[2:6] = struct.pack('>I', 25)  # Root directory LBA
        pt[6:8] = struct.pack('>H', 1)  # Parent directory number
        pt[8] = 0  # Directory identifier
        
        # Padding
        pt[9:2048] = b'\x00' * (2048 - 9)
        
        return bytes(pt)
    
    def _create_root_directory(self):
        """ISO 9660 Root Directory oluştur"""
        root = bytearray(2048)
        
        offset = 0
        
        # Dot entry (.)
        dot_record = bytearray(34)
        dot_record[0] = 34  # Directory Record Length
        dot_record[1] = 0x00  # Extended Attribute Record Length
        dot_record[2:10] = struct.pack('<Q', 25 * 2048)  # LBA (root directory itself)
        dot_record[10:18] = struct.pack('<Q', 2048)  # Data length
        dot_record[18:25] = struct.pack('<7B', 125, 1, 1, 0, 0, 0, 0)
        dot_record[25:33] = struct.pack('<8B', 125, 1, 1, 0, 0, 0, 0, 0)
        dot_record[33] = 0  # File identifier length (0 = dot)
        
        root[offset:offset+34] = dot_record
        offset += 34
        
        # DotDot entry (..)
        dotdot_record = bytearray(34)
        dotdot_record[0] = 34
        dotdot_record[1] = 0x00
        dotdot_record[2:10] = struct.pack('<Q', 25 * 2048)  # LBA
        dotdot_record[10:18] = struct.pack('<Q', 2048)  # Data length
        dotdot_record[18:25] = struct.pack('<7B', 125, 1, 1, 0, 0, 0, 0)
        dotdot_record[25:33] = struct.pack('<8B', 125, 1, 1, 0, 0, 0, 0, 0)
        dotdot_record[33] = 1  # File identifier length (1 = dotdot)
        
        root[offset:offset+34] = dotdot_record
        offset += 34
        
        # Padding
        root[offset:] = b'\x00' * (2048 - offset)
        
        return bytes(root)
    
    def _get_iso_timestamp(self):
        """ISO 8601 format timestamp oluştur (17 bytes)"""
        import datetime
        now = datetime.datetime.now()
        
        # Format: YYYYMMDDHHmmssff (17 bytes)
        ts = f"{now.year:04d}{now.month:02d}{now.day:02d}{now.hour:02d}{now.minute:02d}{now.second:02d}00"
        
        # 17. byte = timezone offset
        return ts.encode('ascii') + b'\x00'
    
    def build(self):
        """Tüm derleme sürecini yap"""
        print("\n" + "=" * 60)
        print("CaYaDev Recovery System - Build")
        print("=" * 60 + "\n")
        
        # Kontroller
        if not self.check_nasm():
            return False
        
        # Derleme
        if not self.assemble_bootloader():
            return False
        
        if not self.assemble_kernel():
            return False
        
        # ISO oluştur
        if not self.create_iso():
            return False
        
        # Başarı mesajı
        print("\n" + "=" * 60)
        print("✓ Derleme başarılı!")
        print("=" * 60)
        print(f"\nBootloader: {self.boot_bin}")
        print(f"Kernel:     {self.kernel_bin}")
        print(f"ISO:        {self.iso_image}")
        print(f"\nBoot etmek için:")
        print(f"  qemu-system-i386 -cdrom {self.iso_image}")
        print(f"\nUSB'ye yazmak için (Linux):")
        print(f"  sudo dd if={self.iso_image} of=/dev/sdX bs=4M status=progress")
        print()
        
        return True

def main():
    # Doğru project root'u bul
    script_path = Path(__file__).resolve()
    project_root = script_path.parent
    
    print(f"[DEBUG] Script: {script_path}")
    print(f"[DEBUG] Project root: {project_root}")
    print(f"[DEBUG] src dizini: {project_root / 'src'}")
    print()
    
    builder = CaYaBuilder(project_root)
    
    success = builder.build()
    sys.exit(0 if success else 1)

if __name__ == "__main__":
    main()

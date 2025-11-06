# CaYaDev Recovery System - Makefile
# Derleme ve yönetim için

.PHONY: all build clean test run debug help install-deps

NASM := nasm
PYTHON := python3
QEMU := qemu-system-i386

# Dosya yolları
SRC_DIR := src
BUILD_DIR := build
DOCS_DIR := docs

# Çıktı dosyaları
BOOT_BIN := $(BUILD_DIR)/boot.bin
KERNEL_BIN := $(BUILD_DIR)/kernel.bin
ISO_IMAGE := $(BUILD_DIR)/CaYaRecovery.iso

# Kaynak dosyalar
BOOT_SRC := $(SRC_DIR)/bootloader/boot.asm
KERNEL_SRC := $(SRC_DIR)/kernel/kernel.asm

# ============================================================
# ANA HEDEFLER
# ============================================================

all: build

# Tümünü derle
build: check-deps $(BOOT_BIN) $(KERNEL_BIN) $(ISO_IMAGE)
	@echo "[SUCCESS] Derleme tamamlandı"
	@echo ""
	@echo "Çıktı dosyaları:"
	@ls -lh $(BUILD_DIR)/
	@echo ""
	@echo "Test: make run"
	@echo "Temizle: make clean"

# ============================================================
# BAĞIMLILIKLARI KONTROL ET
# ============================================================

check-deps:
	@echo "[INFO] Araçlar kontrol ediliyor..."
	@command -v $(NASM) >/dev/null 2>&1 || (echo "[ERROR] NASM bulunamadı. Yüklemek için: sudo apt-get install nasm"; exit 1)
	@command -v $(PYTHON) >/dev/null 2>&1 || (echo "[ERROR] Python3 bulunamadı. Yüklemek için: sudo apt-get install python3"; exit 1)
	@echo "[SUCCESS] Tüm araçlar bulundu"

# ============================================================
# DERLEME HEDEFLERİ
# ============================================================

# Bootloader'ı derle
$(BOOT_BIN): $(BOOT_SRC) check-deps
	@echo "[INFO] Bootloader derleniyorum..."
	@mkdir -p $(BUILD_DIR)
	@$(NASM) -f bin -o $@ $<
	@if [ $$(stat -f%z "$@" 2>/dev/null || stat -c%s "$@") -eq 512 ]; then \
		echo "[SUCCESS] Bootloader derlemesi başarılı ($@)"; \
	else \
		echo "[WARNING] Bootloader boyutu 512 byte değil: $$(stat -f%z "$@" 2>/dev/null || stat -c%s "$@") byte"; \
	fi

# Kernel'i derle
$(KERNEL_BIN): $(KERNEL_SRC) check-deps
	@echo "[INFO] Kernel derleniyorum..."
	@mkdir -p $(BUILD_DIR)
	@$(NASM) -f bin -o $@ $<
	@echo "[SUCCESS] Kernel derlemesi başarılı ($@)"

# ISO'yu oluştur
$(ISO_IMAGE): $(BOOT_BIN) $(KERNEL_BIN)
	@echo "[INFO] ISO dosyası oluşturuluyor..."
	@$(PYTHON) build.py
	@echo "[SUCCESS] ISO oluşturuldu: $(ISO_IMAGE)"

# ============================================================
# TEST HEDEFLERİ
# ============================================================

# QEMU ile çalıştır
run: build
	@echo "[INFO] QEMU'da ISO başlatılıyor..."
	@command -v $(QEMU) >/dev/null 2>&1 || (echo "[ERROR] QEMU bulunamadı. Yüklemek için: sudo apt-get install qemu-system-x86"; exit 1)
	$(QEMU) -cdrom $(ISO_IMAGE) -m 256 -display gtk

# QEMU'da, terminal'de çalıştır (Ctrl+A X ile çık)
run-terminal: build
	$(QEMU) -cdrom $(ISO_IMAGE) -m 256 -nographic

# QEMU'da debug modu
debug: build
	$(QEMU) -cdrom $(ISO_IMAGE) -m 256 -display gtk -gdb tcp::1234 -S
	@echo "GDB ile bağlan: gdb"
	@echo "  (gdb) target remote :1234"
	@echo "  (gdb) continue"

# Bootloader'ı kontrol et
check-boot: $(BOOT_BIN)
	@echo "[INFO] Bootloader kontrol ediliyor..."
	@echo "Boyut:"
	@stat -f%z "$(BOOT_BIN)" 2>/dev/null || stat -c%s "$(BOOT_BIN)"
	@echo ""
	@echo "Boot Signature:"
	@hexdump -C $(BOOT_BIN) | tail -1
	@echo "  (0x55AA olmalı)"

# Kernel'i kontrol et
check-kernel: $(KERNEL_BIN)
	@echo "[INFO] Kernel kontrol ediliyor..."
	@echo "Boyut:"
	@stat -f%z "$(KERNEL_BIN)" 2>/dev/null || stat -c%s "$(KERNEL_BIN)"

# ISO'yu kontrol et
check-iso: $(ISO_IMAGE)
	@echo "[INFO] ISO kontrol ediliyor..."
	@file $(ISO_IMAGE)
	@echo "Boyut:"
	@stat -f%z "$(ISO_IMAGE)" 2>/dev/null || stat -c%s "$(ISO_IMAGE)"

# ============================================================
# TEMIZLEME
# ============================================================

clean:
	@echo "[INFO] Dosyalar temizleniyor..."
	@rm -rf $(BUILD_DIR)/*.bin $(BUILD_DIR)/*.iso $(BUILD_DIR)/iso_temp
	@echo "[SUCCESS] Temizlik tamamlandı"

clean-all: clean
	@echo "[INFO] Tüm build dosyaları siliniyor..."
	@rm -rf $(BUILD_DIR)
	@echo "[SUCCESS] Tümü temizlendi"

# ============================================================
# KURULUM HEDEFLERİ
# ============================================================

install-deps:
	@echo "[INFO] Bağımlılıklar yükleniyor..."
	@echo "[INFO] Linux (Debian/Ubuntu):"
	@echo "  sudo apt-get update"
	@echo "  sudo apt-get install -y nasm python3 qemu-system-x86"
	@echo ""
	@echo "[INFO] macOS:"
	@echo "  brew install nasm qemu"
	@echo ""
	@echo "Sistemine uygun komutu çalıştır."

# ============================================================
# DOKÜMANTASYON HEDEFLERİ
# ============================================================

docs:
	@echo "[INFO] Dokümantasyon dosyaları:"
	@ls -lh $(DOCS_DIR)/

view-docs:
	@echo "[INFO] ARCHITECTURE.md açılıyor..."
	@cat $(DOCS_DIR)/ARCHITECTURE.md | less

# ============================================================
# GERİ VE İLERİ ADIMLAR
# ============================================================

# Sadece bootloader
boot: $(BOOT_BIN)
	@echo "[SUCCESS] Bootloader: $(BOOT_BIN)"

# Sadece kernel
kernel: $(KERNEL_BIN)
	@echo "[SUCCESS] Kernel: $(KERNEL_BIN)"

# Sadece ISO
iso: $(ISO_IMAGE)
	@echo "[SUCCESS] ISO: $(ISO_IMAGE)"

# ============================================================
# USB YAZMA
# ============================================================

usb:
	@echo "[INFO] USB yazma seçenekleri:"
	@echo ""
	@echo "Linux'ta:"
	@echo "  1. lsblk komutu ile USB sürücüyü bul (örn: /dev/sdb)"
	@echo "  2. sudo dd if=$(ISO_IMAGE) of=/dev/sdbX bs=4M status=progress"
	@echo ""
	@echo "Windows'ta:"
	@echo "  - Rufus: https://rufus.ie/"
	@echo "  - Ventoy: https://www.ventoy.net/"

# ============================================================
# YÖNETİM
# ============================================================

stats:
	@echo "[INFO] Proje istatistikleri:"
	@echo ""
	@echo "Bootloader:"
	@wc -l $(BOOT_SRC)
	@echo ""
	@echo "Kernel:"
	@wc -l $(KERNEL_SRC)
	@echo ""
	@echo "Toplamda:"
	@find $(SRC_DIR) -name "*.asm" | xargs wc -l | tail -1

size:
	@echo "[INFO] Dosya boyutları:"
	@echo ""
	@if [ -f $(BOOT_BIN) ]; then echo "Boot.bin: $$(stat -f%z "$(BOOT_BIN)" 2>/dev/null || stat -c%s "$(BOOT_BIN)") byte"; fi
	@if [ -f $(KERNEL_BIN) ]; then echo "Kernel.bin: $$(stat -f%z "$(KERNEL_BIN)" 2>/dev/null || stat -c%s "$(KERNEL_BIN)") byte"; fi
	@if [ -f $(ISO_IMAGE) ]; then echo "ISO: $$(stat -f%z "$(ISO_IMAGE)" 2>/dev/null || stat -c%s "$(ISO_IMAGE)") byte"; fi

# ============================================================
# YARDIM
# ============================================================

help:
	@echo "CaYaDev Recovery System - Makefile"
	@echo ""
	@echo "Temel komutlar:"
	@echo "  make build          - Tümünü derle (default)"
	@echo "  make clean          - Derlenmiş dosyaları sil"
	@echo "  make run            - QEMU'da çalıştır"
	@echo ""
	@echo "Test komutları:"
	@echo "  make check-boot     - Bootloader'ı kontrol et"
	@echo "  make check-kernel   - Kernel'i kontrol et"
	@echo "  make check-iso      - ISO'yu kontrol et"
	@echo ""
	@echo "Ayrı hedefler:"
	@echo "  make boot           - Sadece bootloader derle"
	@echo "  make kernel         - Sadece kernel derle"
	@echo "  make iso            - Sadece ISO oluştur"
	@echo ""
	@echo "Diğer komutlar:"
	@echo "  make install-deps   - Bağımlılıkları yükle"
	@echo "  make docs           - Dokümantasyon dosyaları"
	@echo "  make stats          - Proje istatistikleri"
	@echo "  make size           - Dosya boyutları"
	@echo "  make usb            - USB yazma talimatları"
	@echo "  make help           - Bu yardım (default target)"

# ============================================================
# DEFAULT HEDEF
# ============================================================

.DEFAULT_GOAL := help

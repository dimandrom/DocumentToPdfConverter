# Установка на Linux (сервер и рабочие станции) / Installation on Linux (server and workstations)

---

## Русский

### Ubuntu Server / Debian (сервер или локальная машина)

Минимальный набор LibreOffice для headless-конвертации (без GUI):

```bash
sudo apt update
sudo apt install -y libreoffice-writer libreoffice-calc libreoffice-impress libreoffice-core-nogui --no-install-recommends
```

Проверка:

```bash
/usr/bin/soffice --version
```

Если пакет `libreoffice-core-nogui` недоступен в вашей версии:

```bash
sudo apt install -y libreoffice-writer libreoffice-calc libreoffice-impress --no-install-recommends
```

Полная установка (если нужен полный LibreOffice, в том числе для десктопа):

```bash
sudo apt install -y libreoffice
```

### Несколько серверов Linux (группа серверов)

На каждом сервере выполните те же команды. Для автоматизации можно использовать:

- **Ansible** — плейбук с установкой пакетов `libreoffice-writer`, `libreoffice-calc`, `libreoffice-impress` (и при необходимости `libreoffice-core-nogui`).
- **Скрипт по SSH** — цикл по списку хостов и выполнение `apt install ...`.
- **Образ/шаблон** — предустановленный LibreOffice в образе VM или контейнера (Docker с LibreOffice внутри).

Переменная окружения для приложения (если `soffice` не в PATH):

```bash
export SOFFICE_PATH=/usr/bin/soffice
```

В systemd-сервисе добавьте в `[Service]`:

```ini
Environment="SOFFICE_PATH=/usr/bin/soffice"
```

### RHEL / CentOS / Rocky / Fedora

```bash
# RHEL/CentOS/Rocky (dnf или yum)
sudo dnf install -y libreoffice-writer libreoffice-calc libreoffice-impress libreoffice-core --noinstallrecommends

# или полная установка
sudo dnf install -y libreoffice
```

Проверка: `soffice --version` (обычно `/usr/bin/soffice`).

### Локальные машины с графикой (Ubuntu/Debian desktop)

Тот же набор пакетов, что и для сервера, или полная установка:

```bash
sudo apt install -y libreoffice
```

Путь к `soffice`: `/usr/bin/soffice` или `/usr/bin/libreoffice` (симлинк).

---

## English

### Ubuntu Server / Debian (server or local machine)

Minimal LibreOffice for headless conversion (no GUI):

```bash
sudo apt update
sudo apt install -y libreoffice-writer libreoffice-calc libreoffice-impress libreoffice-core-nogui --no-install-recommends
```

Check:

```bash
/usr/bin/soffice --version
```

If `libreoffice-core-nogui` is not available in your release:

```bash
sudo apt install -y libreoffice-writer libreoffice-calc libreoffice-impress --no-install-recommends
```

Full install (desktop or full suite):

```bash
sudo apt install -y libreoffice
```

### Multiple Linux servers (server group)

Run the same commands on each server. To automate:

- **Ansible** — playbook installing `libreoffice-writer`, `libreoffice-calc`, `libreoffice-impress` (and optionally `libreoffice-core-nogui`).
- **SSH script** — loop over hosts and run `apt install ...`.
- **Image/template** — preinstall LibreOffice in your VM or container image (e.g. Docker with LibreOffice inside).

If `soffice` is not on PATH:

```bash
export SOFFICE_PATH=/usr/bin/soffice
```

For a systemd service, in `[Service]`:

```ini
Environment="SOFFICE_PATH=/usr/bin/soffice"
```

### RHEL / CentOS / Rocky / Fedora

```bash
# RHEL/CentOS/Rocky (dnf or yum)
sudo dnf install -y libreoffice-writer libreoffice-calc libreoffice-impress libreoffice-core --noinstallrecommends

# or full install
sudo dnf install -y libreoffice
```

Check: `soffice --version` (typically `/usr/bin/soffice`).

### Local workstations (Ubuntu/Debian desktop)

Same packages as server, or full install:

```bash
sudo apt install -y libreoffice
```

Path: `/usr/bin/soffice` or `/usr/bin/libreoffice` (symlink).

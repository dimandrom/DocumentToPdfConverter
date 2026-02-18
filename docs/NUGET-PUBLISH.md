# Публикация пакета в NuGet.org / Publishing the package to NuGet.org

---

## Русский

### Как пакет попадает в NuGet

1. **Сборка и упаковка** выполняются в GitHub Actions (workflow `NuGet Pack and Publish`).
2. **Артефакт** — собранный `.nupkg` сохраняется как артефакт запуска workflow (скачать можно со вкладки Run).
3. **Публикация на NuGet.org** — при выполнении условий (тег `v*` или ручной запуск с опцией «Publish to NuGet.org») пакет отправляется на https://www.nuget.org/, если в репозитории задан секрет `NUGET_API_KEY`.

### Что нужно сделать один раз

#### 1. Получить API-ключ NuGet.org

1. Зайдите на https://www.nuget.org/ и войдите в аккаунт (или зарегистрируйтесь).
2. Откройте **Account** → **API Keys** (или https://www.nuget.org/account/apikeys).
3. **Create** → укажите имя (например, `GitHub DocumentToPdfConverter`), в **Select Scopes** выберите **Push** (Push new packages and package versions).
4. В **Select Packages** можно ограничить ключ одним пакетом (например, `DocumentToPdfConverter`) или оставить **All**.
5. Нажмите **Create**, скопируйте ключ (он показывается один раз).

#### 2. Добавить секрет в GitHub

1. Репозиторий на GitHub → **Settings** → **Secrets and variables** → **Actions**.
2. **New repository secret**:
   - **Name:** `NUGET_API_KEY`
   - **Value:** вставьте скопированный API-ключ NuGet.org.
3. Сохраните.

### Как запускать пайплайн

#### Вариант A: публикация по тегу (релиз)

1. Убедитесь, что в репозитории есть коммиты в `main` и все изменения закоммичены.
2. Создайте тег версии (например, `v1.0.0`):
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. В GitHub откройте **Actions** → workflow **NuGet Pack and Publish** — запустится автоматически.
4. Версия пакета возьмётся из тега (`1.0.0`). После успешного выполнения пакет будет собран, загружен как артефакт и отправлен на NuGet.org (если задан `NUGET_API_KEY`).

#### Вариант B: ручной запуск (без тега)

1. **Actions** → **NuGet Pack and Publish** → **Run workflow**.
2. При необходимости укажите **Package version** (например, `1.0.1`). Если оставить пустым, будет использована версия из `.csproj` (по умолчанию `1.0.0`).
3. Включите **Publish to NuGet.org** только если нужно отправить пакет на nuget.org в этом запуске.
4. Нажмите **Run workflow**.

Артефакт с `.nupkg` можно скачать из завершённого запуска (вкладка **Artifacts**).

### После публикации

- Пакет появится на https://www.nuget.org/packages/DocumentToPdfConverter/ (возможна задержка 1–5 минут).
- Установка: `dotnet add package DocumentToPdfConverter`.
- Повторная публикация той же версии запрещена: для выкладки обновления увеличьте версию (новый тег или новое значение в ручном запуске).

---

## English

### How the package gets to NuGet

1. **Build and pack** are done in GitHub Actions (workflow **NuGet Pack and Publish**).
2. **Artifact** — the built `.nupkg` is uploaded as a workflow run artifact (download from the Run page).
3. **Publish to NuGet.org** — when the workflow runs on a `v*` tag or manually with “Publish to NuGet.org” enabled, the package is pushed to https://www.nuget.org/ if the repository secret `NUGET_API_KEY` is set.

### One-time setup

#### 1. Get a NuGet.org API key

1. Sign in at https://www.nuget.org/ (or create an account).
2. Go to **Account** → **API Keys** (or https://www.nuget.org/account/apikeys).
3. **Create** — enter a name (e.g. `GitHub DocumentToPdfConverter`), under **Select Scopes** choose **Push** (Push new packages and package versions).
4. Under **Select Packages** you can limit the key to one package (e.g. `DocumentToPdfConverter`) or leave **All**.
5. Click **Create** and copy the key (it is shown only once).

#### 2. Add the secret in GitHub

1. In the GitHub repo go to **Settings** → **Secrets and variables** → **Actions**.
2. **New repository secret**:
   - **Name:** `NUGET_API_KEY`
   - **Value:** paste the NuGet.org API key.
3. Save.

### How to run the pipeline

#### Option A: Publish via tag (release)

1. Ensure your changes are committed and pushed to `main`.
2. Create a version tag (e.g. `v1.0.0`):
   ```bash
   git tag v1.0.0
   git push origin v1.0.0
   ```
3. In GitHub open **Actions** → **NuGet Pack and Publish** — the workflow will run automatically.
4. Package version is taken from the tag (`1.0.0`). On success the package is built, uploaded as an artifact, and pushed to NuGet.org (if `NUGET_API_KEY` is set).

#### Option B: Manual run (no tag)

1. **Actions** → **NuGet Pack and Publish** → **Run workflow**.
2. Optionally set **Package version** (e.g. `1.0.1`). If left empty, the version from `.csproj` is used (default `1.0.0`).
3. Enable **Publish to NuGet.org** only if you want to push to nuget.org in this run.
4. Click **Run workflow**.

You can download the `.nupkg` artifact from the completed run (**Artifacts** section).

### After publishing

- The package will appear at https://www.nuget.org/packages/DocumentToPdfConverter/ (may take 1–5 minutes).
- Install with: `dotnet add package DocumentToPdfConverter`.
- You cannot push the same version again; bump the version (new tag or new value in manual run) for updates.

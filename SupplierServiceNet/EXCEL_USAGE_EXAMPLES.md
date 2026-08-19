# Excel Endpoints - Usage Examples

## Quick Start Guide

### Step 1: Download Template

```bash
# Using curl
curl -X GET https://localhost:7008/api/v1/suppliers/export/template \
  -H "Authorization: Bearer eyJhbGc..." \
  --output template.xlsx
```

**Result:** `Supplier_Template.xlsx` file with structure:

```
┌─────────────────┬──────────────────┬─────────────────────┬──────────────┬───────────────────┐
│ ID              │ Name *           │ Email *             │ Phone *      │ Address           │
├─────────────────┼──────────────────┼─────────────────────┼──────────────┼───────────────────┤
│ (Leave empty)   │ Example Company  │ contact@example.com │ +1234567890  │ 123 Main Street   │
├─────────────────┼──────────────────┼─────────────────────┼──────────────┼───────────────────┤
│                 │                  │                     │              │                   │
├─────────────────┼──────────────────┼─────────────────────┼──────────────┼───────────────────┤
│                 │                  │                     │              │                   │
```

---

### Step 2: Fill Template & Save

**Example data to add:**

```
┌──────────────────────────────────┬──────────────────────┬──────────────────────┬──────────────┬────────────────────────┐
│ ID                               │ Name                 │ Email                │ Phone        │ Address                │
├──────────────────────────────────┼──────────────────────┼──────────────────────┼──────────────┼────────────────────────┤
│ (leave empty for new)            │ Acme Corp            │ info@acme.com        │ +1-555-0001  │ 100 Broadway, NY        │
├──────────────────────────────────┼──────────────────────┼──────────────────────┼──────────────┼────────────────────────┤
│ (leave empty for new)            │ TechStart Inc        │ contact@techstart.io │ +1-555-0002  │ 200 Silicon Valley, CA  │
├──────────────────────────────────┼──────────────────────┼──────────────────────┼──────────────┼────────────────────────┤
│ 123e4567-e89b-12d3-a456-42661417 │ Global Systems       │ sales@global.net     │ +1-555-0003  │ 300 Commerce Dr, TX     │
```

**Save as:** `suppliers_import.xlsx`

---

### Step 3: Upload File

```bash
# Using curl
curl -X POST https://localhost:7008/api/v1/suppliers/import \
  -H "Authorization: Bearer eyJhbGc..." \
  -F "file=@suppliers_import.xlsx"
```

**Response:**

```json
{
  "totalRows": 3,
  "successfulImports": 3,
  "failedImports": 0,
  "errors": [],
  "message": "Successfully imported 3 suppliers"
}
```

---

## Detailed Examples

### Example 1: Create New Suppliers via Import

**File Content:**
```
ID | Name            | Email               | Phone      | Address
---|-----------------|---------------------|------------|------------------
   | ABC Trading     | contact@abc.com     | 555-0101   | 101 Market St
   | XYZ Solutions   | info@xyz-sol.com    | 555-0102   | 102 Park Ave
   | DEF Consulting  | sales@def.com       | 555-0103   | 103 Tech Dr
```

**Command:**
```bash
curl -X POST https://localhost:7008/api/v1/suppliers/import \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@new_suppliers.xlsx"
```

**Response:**
```json
{
  "totalRows": 3,
  "successfulImports": 3,
  "failedImports": 0,
  "errors": [],
  "message": "Successfully imported 3 suppliers"
}
```

**Result:** 3 new suppliers created with status "Pending"

---

### Example 2: Update Existing Suppliers

**File Content (exported data with modifications):**
```
ID                               | Name                  | Email              | Phone      | Address
---------------------------------|----------------------|-------------------|------------|------------------
00000000-0000-0000-0000-000000001 | ABC Trading Updated   | contact@abc.com    | 555-0111   | 201 Market St
00000000-0000-0000-0000-000000002 | XYZ Solutions Inc     | info@xyz-sol.com   | 555-0112   | 202 Park Ave
   | New Vendor Corp                   | vendor@newco.com   | 555-0113   | 203 Commerce Bl
```

**Command:**
```bash
curl -X POST https://localhost:7008/api/v1/suppliers/import \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@suppliers_updated.xlsx"
```

**Response:**
```json
{
  "totalRows": 3,
  "successfulImports": 3,
  "failedImports": 0,
  "errors": [],
  "message": "Successfully imported 3 suppliers"
}
```

**Result:**
- Row 1: ABC Trading updated (name + address)
- Row 2: XYZ Solutions updated (name)
- Row 3: New Vendor Corp created

---

### Example 3: Import with Validation Errors

**File Content (with errors):**
```
ID | Name            | Email                | Phone      | Address
---|-----------------|----------------------|------------|------------------
   | BadEmail Corp   | invalid-email-here   | 555-0201   | 401 Error Way
   | Missing Phone   | email@nophone.com    |            | 402 Void Dr
   | All Good        | perfect@email.com    | 555-0202   | 403 Success Ln
```

**Command:**
```bash
curl -X POST https://localhost:7008/api/v1/suppliers/import \
  -H "Authorization: Bearer $TOKEN" \
  -F "file=@suppliers_errors.xlsx"
```

**Response:**
```json
{
  "totalRows": 3,
  "successfulImports": 1,
  "failedImports": 2,
  "errors": [
    {
      "rowNumber": 2,
      "identifier": "BadEmail Corp",
      "errors": [
        "Email format is invalid"
      ],
      "errorSummary": "Email format is invalid"
    },
    {
      "rowNumber": 3,
      "identifier": "Missing Phone",
      "errors": [
        "Phone is required"
      ],
      "errorSummary": "Phone is required"
    }
  ],
  "message": "Imported 1 of 3 suppliers. 2 failed."
}
```

**Result:** 
- Row 1: Failed - invalid email
- Row 2: Failed - missing phone
- Row 3: Successful - created "All Good" supplier

---

### Example 4: Export Current Data

**Command:**
```bash
curl -X GET https://localhost:7008/api/v1/suppliers/export \
  -H "Authorization: Bearer $TOKEN" \
  --output export_20260819.xlsx
```

**File Received:** `Suppliers_Export_20260819_120000.xlsx`

**Content:**
```
ID                               | Name            | Email              | Phone      | Address        | Status   | Created By     | Created At
---------------------------------|-----------------|-------------------|------------|----------------|----------|----------------|----------------------
00000000-0000-0000-0000-000000001 | ABC Trading     | contact@abc.com    | 555-0101   | 101 Market St  | Approved | raul@stracon   | 2026-08-15 10:30:00
00000000-0000-0000-0000-000000002 | XYZ Solutions   | info@xyz-sol.com   | 555-0102   | 102 Park Ave   | Pending  | maria@stracon  | 2026-08-18 14:45:00
00000000-0000-0000-0000-000000003 | DEF Consulting  | sales@def.com      | 555-0103   | 103 Tech Dr    | Approved | carlos@stracon | 2026-08-17 09:15:00
```

---

## Client Implementation Examples

### C# Example

```csharp
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

class ExcelExampleClient
{
    private readonly string _baseUrl = "https://localhost:7008";
    private readonly string _token = "YOUR_JWT_TOKEN";
    private readonly HttpClient _httpClient;

    public ExcelExampleClient()
    {
        _httpClient = new HttpClient();
        _httpClient.DefaultRequestHeaders.Authorization = 
            new AuthenticationHeaderValue("Bearer", _token);
    }

    // Download Template
    public async Task DownloadTemplate()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/suppliers/export/template");
        var fileBytes = await response.Content.ReadAsByteArrayAsync();
        await System.IO.File.WriteAllBytesAsync("template.xlsx", fileBytes);
        Console.WriteLine("✓ Template downloaded");
    }

    // Export Data
    public async Task ExportSuppliers()
    {
        var response = await _httpClient.GetAsync($"{_baseUrl}/api/v1/suppliers/export");
        var fileBytes = await response.Content.ReadAsByteArrayAsync();
        var fileName = $"export_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
        await System.IO.File.WriteAllBytesAsync(fileName, fileBytes);
        Console.WriteLine($"✓ Data exported to {fileName}");
    }

    // Import Data
    public async Task ImportSuppliers(string filePath)
    {
        using var content = new MultipartFormDataContent();
        var fileStream = System.IO.File.OpenRead(filePath);
        var fileContent = new StreamContent(fileStream);
        fileContent.Headers.ContentType = 
            new System.Net.Http.Headers.MediaTypeHeaderValue(
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        
        content.Add(fileContent, "file", System.IO.Path.GetFileName(filePath));

        var response = await _httpClient.PostAsync($"{_baseUrl}/api/v1/suppliers/import", content);
        var result = await response.Content.ReadAsStringAsync();
        
        Console.WriteLine("Import Result:");
        Console.WriteLine(result);
    }
}

// Usage
class Program
{
    static async Task Main()
    {
        var client = new ExcelExampleClient();
        
        // Download template
        await client.DownloadTemplate();
        
        // Export current data
        await client.ExportSuppliers();
        
        // Import new data
        await client.ImportSuppliers("suppliers.xlsx");
    }
}
```

### PowerShell Example

```powershell
# Configuration
$baseUrl = "https://localhost:7008"
$token = "YOUR_JWT_TOKEN"
$headers = @{
    "Authorization" = "Bearer $token"
}

# Download Template
function Get-SupplierTemplate {
    $uri = "$baseUrl/api/v1/suppliers/export/template"
    $response = Invoke-WebRequest -Uri $uri -Headers $headers -OutFile "template.xlsx"
    Write-Host "✓ Template downloaded"
}

# Export Data
function Export-Suppliers {
    $uri = "$baseUrl/api/v1/suppliers/export"
    $fileName = "export_$(Get-Date -Format 'yyyyMMdd_HHmmss').xlsx"
    Invoke-WebRequest -Uri $uri -Headers $headers -OutFile $fileName
    Write-Host "✓ Data exported to $fileName"
}

# Import Data
function Import-Suppliers {
    param(
        [Parameter(Mandatory)]
        [string]$FilePath
    )
    
    $uri = "$baseUrl/api/v1/suppliers/import"
    $form = @{
        file = Get-Item -Path $FilePath
    }
    
    $response = Invoke-WebRequest -Uri $uri -Method POST -Headers $headers -Form $form
    $result = $response.Content | ConvertFrom-Json
    
    Write-Host "Import Result:"
    Write-Host "  Total Rows: $($result.totalRows)"
    Write-Host "  Successful: $($result.successfulImports)"
    Write-Host "  Failed: $($result.failedImports)"
    Write-Host "  Message: $($result.message)"
    
    if ($result.errors.Count -gt 0) {
        Write-Host ""
        Write-Host "Errors:"
        $result.errors | ForEach-Object {
            Write-Host "  Row $($_.rowNumber): $($_.errorSummary)"
        }
    }
}

# Usage
Get-SupplierTemplate
Export-Suppliers
Import-Suppliers -FilePath "suppliers_to_import.xlsx"
```

### JavaScript/Node.js Example

```javascript
const axios = require('axios');
const FormData = require('form-data');
const fs = require('fs');

const API_BASE = 'https://localhost:7008';
const TOKEN = 'YOUR_JWT_TOKEN';

const client = axios.create({
    baseURL: API_BASE,
    headers: {
        'Authorization': `Bearer ${TOKEN}`
    }
});

// Download Template
async function downloadTemplate() {
    try {
        const response = await client.get('/api/v1/suppliers/export/template', {
            responseType: 'arraybuffer'
        });
        fs.writeFileSync('template.xlsx', response.data);
        console.log('✓ Template downloaded');
    } catch (error) {
        console.error('Error:', error.message);
    }
}

// Export Data
async function exportSuppliers() {
    try {
        const response = await client.get('/api/v1/suppliers/export', {
            responseType: 'arraybuffer'
        });
        const fileName = `export_${new Date().toISOString().split('T')[0]}.xlsx`;
        fs.writeFileSync(fileName, response.data);
        console.log(`✓ Data exported to ${fileName}`);
    } catch (error) {
        console.error('Error:', error.message);
    }
}

// Import Data
async function importSuppliers(filePath) {
    try {
        const form = new FormData();
        form.append('file', fs.createReadStream(filePath));

        const response = await client.post('/api/v1/suppliers/import', form, {
            headers: form.getHeaders()
        });

        const result = response.data;
        console.log('Import Result:');
        console.log(`  Total Rows: ${result.totalRows}`);
        console.log(`  Successful: ${result.successfulImports}`);
        console.log(`  Failed: ${result.failedImports}`);
        console.log(`  Message: ${result.message}`);

        if (result.errors && result.errors.length > 0) {
            console.log('\nErrors:');
            result.errors.forEach(err => {
                console.log(`  Row ${err.rowNumber}: ${err.errorSummary}`);
            });
        }
    } catch (error) {
        console.error('Error:', error.message);
    }
}

// Usage
(async () => {
    await downloadTemplate();
    await exportSuppliers();
    await importSuppliers('suppliers.xlsx');
})();
```

---

## Integration with Postman

### Setup

1. **Create Environment Variable:**
   - Variable: `jwt_token`
   - Value: Your JWT token from login endpoint

2. **Collection Setup:**

```
📁 Suppliers - Excel
├── 📌 Get Template
│   GET {{base_url}}/api/v1/suppliers/export/template
│   Auth: Bearer {{jwt_token}}
│   Send → Save as "template.xlsx"
│
├── 📌 Export Data
│   GET {{base_url}}/api/v1/suppliers/export
│   Auth: Bearer {{jwt_token}}
│   Send → Save as "export.xlsx"
│
└── 📌 Import Data
    POST {{base_url}}/api/v1/suppliers/import
    Auth: Bearer {{jwt_token}}
    Body: form-data
      file: [Select supplier file]
    Send → Review results
```

### Postman Steps

**For Template:**
1. Click "Get Template" request
2. Send
3. In response, click "Save Response" → "Save Response to File"
4. Choose `template.xlsx`

**For Import:**
1. Click "Import Data" request
2. In Body tab, select `form-data`
3. Key: `file`, Value: Select your Excel file
4. Send
5. Review JSON response

---

## Common Workflows

### Workflow: Weekly Supplier Sync

```mermaid
graph LR
    A[Monday 9am] -->|Export| B[Get current suppliers]
    B -->|Review| C[Check for changes needed]
    C -->|Modify| D[Update Excel file]
    D -->|Send to Vendor| E[Share with external team]
    E -->|Returns| F[Updated file from vendor]
    F -->|Import| G[POST /import]
    G -->|Results| H[1 created, 2 updated]
```

### Workflow: Bulk Onboarding

```mermaid
graph LR
    A[New Partners Signed] -->|Create| B[Fill Excel template]
    B -->|Validate| C[Check all fields]
    C -->|Upload| D[POST /import]
    D -->|Process| E[Create 50 new suppliers]
    E -->|Approve| F[Status: Pending]
    F -->|Review| G[QA checks data]
    G -->|Approve| H[Change status to Approved]
```

---

## Troubleshooting Guide

### Issue: "File must be in .xlsx format"

**Problem:** Trying to upload CSV or XLS file

**Solutions:**
```bash
# Convert CSV to XLSX (using Python)
pip install openpyxl
python -c "
import csv
from openpyxl import Workbook
wb = Workbook()
ws = wb.active
with open('suppliers.csv') as f:
    for row in csv.reader(f):
        ws.append(row)
wb.save('suppliers.xlsx')
"

# Convert CSV to XLSX (using PowerShell)
Import-Csv suppliers.csv | Export-Excel suppliers.xlsx
```

### Issue: "Email format is invalid"

**Problem:** Email address has incorrect format

**Check:**
```
✓ Correct:  user@company.com
✓ Correct:  contact@company.co.uk
✗ Wrong:    user@company (missing TLD)
✗ Wrong:    user company.com (missing @)
✗ Wrong:    user@company@extra.com (extra @)
```

### Issue: "Supplier with ID X not found"

**Problem:** Invalid GUID or supplier doesn't exist

**Solution:**
```
1. Get correct ID from export
2. Copy-paste full GUID
3. Leave ID empty if creating new
```

### Issue: File seems empty after export

**Problem:** No suppliers in database yet

**Solution:**
```
1. Create suppliers via API first
2. POST /suppliers with supplier data
3. Then export
```

---

## Performance Tips

1. **Large Imports (1000+ rows):**
   - Split into multiple files
   - Upload in batches
   - Monitor server performance

2. **Frequent Exports:**
   - Cache the result if data doesn't change often
   - Export on schedule, not per request

3. **Memory Usage:**
   - Use streaming if available in client library
   - Delete old export files
   - Monitor disk space for log files

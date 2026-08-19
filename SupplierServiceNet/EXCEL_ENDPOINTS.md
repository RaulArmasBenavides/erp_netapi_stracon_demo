# Excel Endpoints Documentation

This document describes the new Excel import/export functionality added to the Suppliers API.

## Endpoints Overview

### 1. Download Template
**GET** `/api/v1/suppliers/export/template`

Downloads an empty Excel template with the structure for importing suppliers.

**Response:**
- `200 OK` - Returns Excel file (.xlsx)
- `401 Unauthorized` - Invalid or missing JWT token
- `403 Forbidden` - User doesn't have required role (Requester or Approver)

**Example:**
```bash
curl -X GET https://localhost:7008/api/v1/suppliers/export/template \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -o Supplier_Template.xlsx
```

**Template Structure:**
| Column | Required | Notes |
|--------|----------|-------|
| ID | No | Leave empty for new suppliers, provide GUID for updates |
| Name | Yes | Supplier company name |
| Email | Yes | Valid email format required |
| Phone | Yes | Phone number |
| Address | No | Address information |

---

### 2. Export Data
**GET** `/api/v1/suppliers/export`

Downloads current suppliers as Excel file with all data populated.

**Response:**
- `200 OK` - Returns Excel file (.xlsx)
- `401 Unauthorized` - Invalid or missing JWT token
- `403 Forbidden` - User doesn't have required role

**Example:**
```bash
curl -X GET https://localhost:7008/api/v1/suppliers/export \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -o Suppliers_Export_20260819_120000.xlsx
```

**Export Columns:**
| Column | Content |
|--------|---------|
| ID | Supplier GUID |
| Name | Supplier name |
| Email | Email address |
| Phone | Phone number |
| Address | Physical address |
| Status | "Approved" or "Pending" |
| Created By | User who created supplier |
| Created At | Creation timestamp |

---

### 3. Bulk Import
**POST** `/api/v1/suppliers/import`

Imports or updates multiple suppliers from an Excel file. Creates new suppliers or updates existing ones based on ID column.

**Request:**
- Content-Type: `multipart/form-data`
- Parameter: `file` (IFormFile, .xlsx format required)

**Response:**
```json
{
  "totalRows": 5,
  "successfulImports": 4,
  "failedImports": 1,
  "errors": [
    {
      "rowNumber": 3,
      "identifier": "invalid-email-supplier",
      "errors": [
        "Email format is invalid"
      ],
      "errorSummary": "Email format is invalid"
    }
  ],
  "message": "Imported 4 of 5 suppliers. 1 failed."
}
```

**Status Codes:**
- `200 OK` - Import completed (with results, may include errors)
- `400 Bad Request` - Invalid file format or content
- `401 Unauthorized` - Invalid or missing JWT token
- `403 Forbidden` - User doesn't have required role

**Example using cURL:**
```bash
curl -X POST https://localhost:7008/api/v1/suppliers/import \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -F "file=@suppliers_to_import.xlsx"
```

**Example using PowerShell:**
```powershell
$token = "YOUR_JWT_TOKEN"
$file = "C:\path\to\suppliers.xlsx"

$headers = @{
    "Authorization" = "Bearer $token"
}

$form = @{
    file = Get-Item -Path $file
}

Invoke-WebRequest -Uri "https://localhost:7008/api/v1/suppliers/import" `
    -Method POST `
    -Headers $headers `
    -Form $form
```

**Example using C#:**
```csharp
using var client = new HttpClient();
var token = "YOUR_JWT_TOKEN";
client.DefaultRequestHeaders.Authorization = 
    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

using var form = new MultipartFormDataContent();
using var fileStream = File.OpenRead("suppliers.xlsx");
var fileContent = new StreamContent(fileStream);
fileContent.Headers.ContentType = 
    new System.Net.Http.Headers.MediaTypeHeaderValue("application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");

form.Add(fileContent, "file", "suppliers.xlsx");

var response = await client.PostAsync(
    "https://localhost:7008/api/v1/suppliers/import", 
    form);

var result = await response.Content.ReadAsStringAsync();
Console.WriteLine(result);
```

---

## Import Behavior

### Creating New Suppliers
- Leave the **ID** column empty
- Fill in required fields: Name, Email, Phone
- Address is optional
- New suppliers are created with "Pending" status

### Updating Existing Suppliers
- Provide the supplier's GUID in the **ID** column
- Update any fields (partial updates supported)
- Non-empty values override existing data
- Status is not changed by import

### Validation Rules
1. **Name** - Required, non-empty string
2. **Email** - Required, must be valid email format
3. **Phone** - Required, non-empty string
4. **Address** - Optional
5. **ID** - If provided, must be valid GUID format

### Error Handling
- Validation errors are reported per row
- One invalid row does NOT stop import of other rows
- Failed rows are returned in response with error details
- Successful imports are saved to database immediately
- Failed imports are not saved

---

## Example Workflows

### Workflow 1: Download Template → Fill → Import

```
1. GET /api/v1/suppliers/export/template
   ↓
2. Open in Excel
3. Fill in supplier data
4. Save file
5. POST /api/v1/suppliers/import with file
   ↓
6. Review response for success/failures
```

### Workflow 2: Export Current Data → Modify → Re-import

```
1. GET /api/v1/suppliers/export
   ↓
2. Open in Excel
3. Edit existing suppliers or add new rows
4. Save file
5. POST /api/v1/suppliers/import with file
   ↓
6. Review response for what changed
```

### Workflow 3: Bulk Create via Template

```
1. Download template: GET /export/template
2. Add 100 new suppliers (leave ID empty)
3. Import: POST /import
   ↓
   Result: 100 suppliers created with generated GUIDs
```

---

## Response Examples

### Successful Import (All Valid)
```json
{
  "totalRows": 3,
  "successfulImports": 3,
  "failedImports": 0,
  "errors": [],
  "message": "Successfully imported 3 suppliers"
}
```

### Partial Success (Some Errors)
```json
{
  "totalRows": 5,
  "successfulImports": 3,
  "failedImports": 2,
  "errors": [
    {
      "rowNumber": 2,
      "identifier": "company-missing-email",
      "errors": [
        "Email is required"
      ],
      "errorSummary": "Email is required"
    },
    {
      "rowNumber": 4,
      "identifier": "00000000-0000-0000-0000-000000000000",
      "errors": [
        "Supplier with ID 00000000-0000-0000-0000-000000000000 not found"
      ],
      "errorSummary": "Supplier with ID 00000000-0000-0000-0000-000000000000 not found"
    }
  ],
  "message": "Imported 3 of 5 suppliers. 2 failed."
}
```

---

## Technical Details

### File Format Requirements
- **Extension**: `.xlsx` (Excel 2007+)
- **Encoding**: UTF-8 (standard Excel)
- **Size**: No explicit limit (depends on server config)

### Performance Considerations
- Bulk import batches all changes into single transaction
- All-or-nothing per-row basis (one error doesn't rollback others)
- Database SaveChanges called once after all validations
- Suitable for 100s of records at once

### Security
- All endpoints require JWT authentication
- Only "Requester" and "Approver" roles can access
- User identity is captured as "CreatedBy" / "ImportedBy"
- No sensitive data exposed in error messages (production mode)

---

## Troubleshooting

### "File must be in .xlsx format"
- Ensure file is Excel 2007+ (.xlsx), not old Excel (.xls) or CSV
- Re-save in Excel if converted from other format

### "Email format is invalid"
- Check email addresses have valid format (user@domain.com)
- Look for spaces, special chars, missing @ or domain

### "Supplier with ID X not found"
- When updating, ID must be exact GUID of existing supplier
- Copy-paste from export file to ensure correct format
- Leave ID empty if creating new supplier

### Empty file error
- Ensure file has at least header row + 1 data row
- Example row with all blanks is skipped automatically

### Unauthorized (401)
- Token may be expired
- Request new token via /login endpoint
- Include "Bearer " prefix in Authorization header

### Forbidden (403)
- User role is not "Requester" or "Approver"
- Contact admin to grant appropriate role

---

## Migration from CSV/Other Format

To convert existing data to Excel format:

**From CSV → Excel (PowerShell):**
```powershell
$csv = Import-Csv "suppliers.csv"
$csv | Export-Excel -Path "suppliers.xlsx" -WorksheetName "Suppliers"
```

**From database → Excel (C#):**
```csharp
// Export existing suppliers
var response = await client.GetAsync("/api/v1/suppliers/export");
var excelBytes = await response.Content.ReadAsByteArrayAsync();
File.WriteAllBytes("suppliers_backup.xlsx", excelBytes);
```

---

## API Version Support

All Excel endpoints are part of **API v1.0**. Routes follow the versioning pattern:
```
/api/v{version}/suppliers/export/template
/api/v{version}/suppliers/export
/api/v{version}/suppliers/import
```

Future versions (v2.0+) may have different endpoints with breaking changes announced in release notes.

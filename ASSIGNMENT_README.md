# Car Dealer Lead Processing Automation - Technical Assignment

## Overview

Build a **production-grade automation system** that processes car dealer leads through a complete pipeline.

**Time:** 4-6 hours max
**Role Focus:** Business automation engineer (NOT QA) - you'll build systems that drive business processes and integrate with platforms like HubSpot.
**Note:** You may change any part of this assignment if you have a good reason, and explain it during the interview.

---

## Business Scenario

**What is a Lead?**
A **lead** is a potential customer who has expressed interest in purchasing a car by providing their contact information and preferences through digital channels. Each lead represents a sales opportunity that needs to be captured, qualified, and routed to the appropriate sales team member for follow-up.

You work for a **car dealership group**. Leads come from digital channels (Facebook, Google Ads, website). Each lead must be automatically processed: validated, enriched, and logged.

**Sample Lead:**
```json
{
    "BranchID": "400",
    "WorkerCode": "910290",
    "AskedCar": "90962_101",
    "FirstName": "ניר",
    "LastName": "פריידי",
    "Email": "customer@example.com",
    "Phone": "0542100319",
    "FromWebSite": "forthing",
    "Area": "1"
}
```

---


## Your Tasks - Build This Pipeline

### Step 1: API Ingestion
- Create a `/api/leads` endpoint
- The endpoint should receive the lead data in the format shown in the **Sample Lead** section above
- Test the endpoint using Postman with the sample lead JSON
- Process lead asynchronously

### Step 2: Validation
- Email: Valid format, no disposable domains
- **Email verification interface:**
  - Add code comments explaining why email verification services are important for business automation.
  - List alternative 3rd party API providers to validate the Email in comments
  - Explain in comments the business benefits
- Phone: Israeli format (05X-XXXXXXX, 10 digits)
- Required: BranchID, FirstName, LastName, Email OR Phone
- BranchID must be numeric

### Step 3: Load Business Rules from Files
- Read `data/branch_config.xlsx` (Excel) - branch details
- Read `data/car_models.txt` (Text) - car models catalog
- Parse both files and use data to enrich leads

**Connecting Keys:**
- Lead's `BranchID` field → Excel's `BranchID` column
- Lead's `AskedCar` field → car_models.txt's `Model ID` field

### Step 4: External API Enrichment
- Call enrichment API (mock provided at `http://mock-api:8001/api/enrich`)
- Combine external API data + local file data (Step 3)
- Implement lead scoring logic (see Business Logic #1 below)
- Handle failures: retry 3x with exponential backoff, 5s timeout
- Don't block pipeline on failure

**Connecting Keys:**
- Lead's `Email` → API request's `email` (lowercase)
- Lead's `Phone` → API request's `phone` (lowercase)
- Lead's `Area` → API request's `area` (lowercase)

### Step 5: Lead Routing & Final Processing
- Implement routing logic (see Business Logic #2 below)
- Create final enriched lead object (see Business Logic #3 below)
- Store processed leads (DB - your choice)

### Step 6: Implement Structured Logging
- Implement structured logging throughout the system
- Log every pipeline stage with essential fields (score, priority, assigned_to, status) 

### Step 7: Infrastructure & Observability
**Docker:**
- `Dockerfile` + `docker-compose.yml`
- Must run with: `docker-compose up --build`

---

## Business Logic Rules (Translate to Code)

**Important:** You must implement these business rules that combine data from **files** + **mock API enrichment**:

### 1. Lead Scoring Logic
Calculate a lead score (0-100) by combining:

**From Mock API enrichment:**
- `lead_priority` = "High" → +40 points
- `lead_priority` = "Medium" → +20 points
- `lead_priority` = "Low" → +0 points
- `email_insights.trust_level` = "High" → +20 points
- `phone_insights.verified` = true → +20 points

**From car_models.txt:**
- Car `category` = "Luxury" → +20 points
- Car `category` = "Electric" → +15 points
- Car `availability` = "In Stock" → +10 points

**Total Score** = Sum of all applicable points (max 100)

### 2. Lead Routing Rules
Based on final score:
- **Score >= 70:** HOT lead → Assign to branch manager (from `branch_config.xlsx`)
- **Score 40-69:** WARM lead → Assign to worker (use lead's `WorkerCode`)
- **Score < 40:** COLD lead → Assign to general pool

### 3. Data Combination Rules
Create final enriched lead object combining:

```json
{
  "original_lead": { ... },
  "branch_info": {
    "branch_id": "400",
    "name": "Tel Aviv Showroom",
    "manager": "David Cohen",
    "region": "Center"
  },
  "car_info": {
    "model_id": "90962_101",
    "model_name": "U-Tour",
    "category": "SUV",
    "price_range": "165,000 - 185,000 ILS"
  },
  "enrichment": {
    "geographic": {"city": "Tel Aviv", "region": "Center"},
    "email_insights": {"trust_level": "High"},
    "phone_insights": {"carrier": "Orange", "verified": true},
    "lead_priority": "High"
  },
  "score": 85,
  "priority": "HOT",
  "assigned_to": "David Cohen",
  "status": "processed"
}
```

### 4. Error Handling Rules
- **Invalid email + no phone:** Reject lead, log reason
- **BranchID not in Excel:** Use default Branch 400
- **Car model not found:** Continue with car_info = null
- **Enrichment API fails:** Continue with enrichment = null, score = 0
- **File read error:** Log error, use defaults

---

## Mock API Provided

The mock API is available when running docker-compose:

**Enrichment:** `POST http://mock-api:8001/api/enrich`
```json
{
  "email": "test@example.com",
  "phone": "0542100319",
  "area": "1"
}
```

**Response:**
```json
{
  "enriched": true,
  "data": {
    "geographic": {"city": "Tel Aviv", "region": "Center"},
    "email_insights": {"customer_type": "B2C", "trust_level": "High"},
    "phone_insights": {"carrier": "Orange/Partner", "verified": true},
    "lead_priority": "High"
  }
}
```

---

## Deliverables

1. **Working code** - runs with `docker-compose up`
2. **SOLUTION_README.md** with:
   - Setup instructions
   - API documentation
   - Design decisions

3. **Sample requests** (curl/Postman)

---

## Test Data Provided

- `data/sample_leads.json` - Valid & invalid lead examples
- `data/branch_config.xlsx` - 4 branches
- `data/car_models.txt` - 9 car models

---

**We look for:**
- ✅ Correct implementation of scoring & routing logic
- ✅ Proper data combination from files + API
- ✅ End-to-end working system
- ✅ Clean, maintainable code
- ✅ Good error handling

---

## Submission

1. Push code to Git repository
2. Share the link
3. Ensure `docker-compose up --build` works on fresh clone
4. Include `SOLUTION_README.md`

---

Good luck! 

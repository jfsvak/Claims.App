# Claims.App

## How to get the code and run it
In a command promt, run the following

	> git clone https://github.com/jfsvak/Claims.App.git

__To run unit tests__

	> dotnet test Claims.App/src/Claims.App.sln

__To run the app__

	> dotnet run Claims.App/src/Claims.App.sln --project Claims.App/src/Claims.Web

__To test the api's__

Use [Postman](https://www.getpostman.com/) (or similar) to POST a block of text to `http://localhost:5001/api/claims/email`.
For example Postman test cases, import `tests/Claims.App.postman_collection.json` into Postman and run as a collection.


## About the Solution
### Assumptions
I assume for now that the only agreed upon contract for text submission is one that follows the example, with the additional constraints mentioned under the section Failure Conditions.

This gives me the following assumptions:
1. The only mandatory xml element is `<total>`
2. The other xml elements are optional and can appear at the most one time. *Multiple xml elements are discussed below in section __Points for Further Design and Development__*

### Design
I have chosen to have two projects (tests are found in `tests/Claims.App.Tests`)

- src/Claims.Web
- src/Claims.Business

#### Claims.Web project
Contains Web API code to handle communication to and from clients. 
A simple `TextPlainInputFormatter.cs` has been added to handle content type 'text/plain' in the submission.
`ClaimsController.cs` contains the exposed api methods.

Apis are made available on:

	POST	/api/claims/email	// accepts a block of text as the body (email) and parses it into a Claim entity (see below)
	GET	 /api/claims		  // gets all the claims. Currently just returns dummy Claim list as persistence is not implemented.

#### Claims.Business project
Contains implementation of:
- Domain model (business entities) - `Claims.Business.Model.*`
- business rules validation of mandatory fields and default values for missing fields - `Claims.Business.Service.*`
- extracting data from xml - `Claims.Business.Util.*`
- parsing Culture sensitive data (e.g. dates and amounts) - `Claims.Business.Util.*`

### Points for Further Design and Development
#### Business Feature
- Having multiple Expenses under one Claim
	- Submitting expenses separately with a Claim ID and have all the totals accumulated under one Expense
- Having multiple the same event (vendor...)
- Making an Event an independent entity and thus be able to submit events independently from expenses/claims

#### Technical Features
- Put texts in language bundles so I18N can be achieved
- Have separate Model layer for Web in order not to expose Business Domain Model to clients
- Persist successfully extracted Claim entities somewhere, e.g. DB

# Notes/Limitations:
- Had to change the date as it was an invalid date. 27. april 2017 is a thursday.
- Parsing of amounts is not strict with regards to number group size. E.g. 1,23.45 will be parsed to an allowed value of 123.45. This is default behaviour for number parsing, so if strict amount formats needs to be followed, validation of this needs to be implemented separately

## TODO:
- DONE fix no opening tag issue
- DONE write doc on classes
- look into XmlExtractor.GetXmlElement for superfluous code
- test page to get text and send to /api/claims/email
- DONE cleanup console.writelines -> Changed to Debug
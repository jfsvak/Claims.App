# Claims.App

## Assumptions
I assume for now that the only agreed upon contract for xml submission is one that follows the example, with the additional constraints mentioned under the section Failure Conditions.

This gives me the following assumptions:
1. The xml element <expense> is always given as an 'island' of xml (will be extracted into an Expense entity with the total value ) 
2. The following xml elements are always given as markup elements (will be placed in an Event entity when extracted)
  - vendor
  - description
  - date


## Business model design

# Points for further design and discussion
- Having multiple Expenses under one Claim
- Having multiple claims under the same event (vendor...)
- Making an Event an independent entity and thus be able to submit events independently from expenses/claims

# Client interface
Client generates its own interface from the running api, using the swagger stuff in Startup.cs

# Notes/Limitations:
- Had to change the date as it was an invalid date. 27. april 2017 is a thursday.
- XmlExtractor.GetString currently has undefined behavior with xml with nested elements with the same name.
  E.g. <vendor><vendor>Outback Steakhouse</vendor></vendor> 
- Parsing of amounts is not strict with regards to number group size. E.g. 1,23.45 will be parsed to an allowed value of 123.45. This is default behaviour for number parsing, so if strict amount formats needs to be followed, validation of this needs to be implemented separately
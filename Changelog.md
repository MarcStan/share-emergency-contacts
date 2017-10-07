**v0.9.5**

* Added Light/dark theme support
    * known issue on android: when launching app with dark theme and switching to light theme icons and tabbedpage text remain white on white theme (fixed by restarting app)
* removed duplicate navbar on UWP
* fixed rare crash on navigating back
* fixed crash on app resume (android only)

**v0.9.4**

* Share details for a defined number of days (your profile is auto. delete from the recipients phone afterwards)
* Export all contacts and profiles at once to a single file (and import from same file)

**v0.9.3**

* introduced upper limit for phone numbers per contact (4)
* introduced upper limit for emergency + insurance contacts per profile (6)
* introduced upper limit for text entered (required due to QR code being limited in size)
* when adding/removing sub contacts from a profile it would not immediately visualize in the saved profile (even though the change was effective)
* fixed layouting for long email and phone numbers
* fixes for iOS/UWP styling
* Added analytics to keep track of feature usage and crashes

**v0.9.2**

* fixed relationship field being too small
* edit dialog now has cancel button with "discard data?" prompt (user can no longer accidently go back and discard data without confirming)
* empty profile can no longer be saved
* made barcode parsing more lax so that more formats are recognized
* generated barcodes more closely follow the spec (better readability for other barcode readers)

**v0.9.1**

* Added additional fields:
  * Allergies
  * Nationality
  * Passport
  * Relationship (of emergency contacts)
* fixed crash on edit profile view
* consistent newlines across platforms

**v0.9.0 (initial)**

* Can add/edit/share own profiles
* Can receive contacts from other people via QR code and share them again
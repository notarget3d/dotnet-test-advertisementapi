To start the service open project in visual studio and run.
Testing is done through Postman. Default address is "http://localhost:5183" You can change this in "AdvertisementApi/Properties/launchSettings.json"
Use endpoint "POST api/adv/add" to upload a file (you can find example files in "AdvertisementApi/Examples/" folder)
Endpoint accepts multiform with "file" key and returns a list of errors found durring parse process.

Use endpoint "GET api/adv/get" to get the list of ad platforms.
Body must contain string array with locations.
Example: ["/ru/svrd/pervik"] or ["/ru", "/ru/msk"]

function ChangeRegistrationRole() {
    const e = document.getElementById("RoleSelect");
    const role = e.options[e.selectedIndex].value;
    switch (role) {
    case "1":
        $("#RightPanelRestaurant").hide(300,
            function () {
                document.getElementById("Latitude").value = 0;
                document.getElementById("Longitude").value = 0;
                $("#RightPanelClient").show(300);
            });
        break;
    case "0":
        $("#RightPanelClient").hide(300,
            function() {
                $("#RightPanelRestaurant").show(300);
            });
        break;
    default:
    }
}

var geocoder;
var map;
var prevMarker;
var lastAddress;
var infowindow;

function clearPreviousMarker() {
    try {
        prevMarker.setMap(null);
    } catch (ex) {
    }
}

function initialize() {
    const latlng = new google.maps.LatLng(52.05, 19.45); //Piątek
    const mapOptions = {
        zoom: 5,
        center: latlng,
        mapTypeId: google.maps.MapTypeId.HYBRID,
        streetViewControl: false,
        mapTypeControl: false,
        maxZoom: 17
    };
    map = new google.maps.Map(document.getElementById("map"), mapOptions);
    geocoder = new google.maps.Geocoder();
}

function GeocodeForm() {
    if (map == null)
        initialize();
    var fewSecsAgoAddress = document.getElementById("InputAddressId").value;
    setTimeout(
        function() {
            const address = document.getElementById("InputAddressId").value;
            if (address.length < 4 || address !== fewSecsAgoAddress)
                return;
            geocoder.geocode({ 'address': address },
                function(results, status) {
                    map.set("zoom", 15);
                    clearPreviousMarker();
                    if (status === google.maps.GeocoderStatus.OK) {
                        const loc = results[0].geometry.location;
                        var components = results[0].address_components;
                        var postalCode = "", country = "", city = "", county = "", province = "", street = "", streetNum = "";
                        $.each(components,
                            function() {
                                if (this.types[0] === "postal_code")
                                    postalCode = this.long_name;
                                if (this.types[0] === "country")
                                    country = this.long_name;
                                if (this.types[0] === "administrative_area_level_1")
                                    province = this.long_name;
                                if (this.types[0] === "administrative_area_level_2")
                                    county = this.long_name;
                                if (this.types[0] === "locality")
                                    city = this.long_name;
                                if (this.types[0] === "street_number" || this.types[0] === "premise")
                                    streetNum = this.long_name;
                                if (this.types[0] === "route")
                                    street = this.long_name;
                            });

                        map.setCenter(loc);
                        prevMarker = new google.maps.Marker({
                            map: map,
                            position: loc
                        });
                        getcontentString(loc, country, city, postalCode, province, county, street, streetNum);
                    } else {
                        //alert("Geocode was not successful for the following reason: " + status);
                    }
                });
        },
        3000);
}

function getcontentString(pos, country, city, postalCode, province, county, street, streetNum) {
    geocoder.geocode({
            latLng: pos
        },
        function(responses) {
            var realAddress;
            if (responses && responses.length > 0)
                realAddress = responses[0].formatted_address;
            else
                realAddress = "Nie można ustalić dokładnego adresu";

            infowindow = new google.maps.InfoWindow({
                content: realAddress
            });
            document.getElementById("RealAddress").value = realAddress;
            document.getElementById("Country").value = country;
            document.getElementById("City").value = city;
            document.getElementById("PostalCode").value = postalCode;
            document.getElementById("Province").value = province;
            document.getElementById("County").value = county;
            document.getElementById("Street").value = street;
            document.getElementById("StreetNum").value = streetNum;
            document.getElementById("Latitude").value = pos.lat();
            document.getElementById("Latitude").value = document.getElementById("Latitude").value.replace(".", ",");
            document.getElementById("Longitude").value = pos.lng();
            document.getElementById("Longitude").value = document.getElementById("Longitude").value.replace(".", ",");
            infowindow.open(map, prevMarker);
            prevMarker.addListener("click",
                function() {
                    infowindow.open(map, prevMarker);
                });
        });
}
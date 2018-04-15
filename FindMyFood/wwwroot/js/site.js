function ChangeRegistrationRole() {
    var e = document.getElementById("RoleSelect");
    var role = e.options[e.selectedIndex].value;
    switch (role) {
    case "1":
        ShowRestaurantClientPanel();
        break;
    case "0":
        ShowRestaurantRegistrationPanel();
        break;
    default:
    }
}

function ShowRestaurantRegistrationPanel() {
    var restaurantPanel = document.getElementById("RightPanelRestaurant");
    var clientPanel = document.getElementById("RightPanelClient");
    restaurantPanel.style.visibility = "visible";
    restaurantPanel.style.height = "";
    restaurantPanel.style.width = "";
    clientPanel.style.visibility = "hidden";
    clientPanel.style.height = "0px";
    clientPanel.style.width = "0px";
    //myMap();
}

function ShowRestaurantClientPanel() {
    var restaurantPanel = document.getElementById("RightPanelRestaurant");
    var clientPanel = document.getElementById("RightPanelClient");
    clientPanel.style.visibility = "visible";
    clientPanel.style.height = "";
    clientPanel.style.width = "";
    restaurantPanel.style.visibility = "hidden";
    restaurantPanel.style.height = "0px";
    restaurantPanel.style.width = "0px";
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
    var latlng = new google.maps.LatLng(52.05, 19.45);//Piątek
    var mapOptions = {
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
            var address = document.getElementById("InputAddressId").value;
            if (address.length < 4 || address !== fewSecsAgoAddress)
                return;
            geocoder.geocode({ 'address': address },
                function(results, status) {
                    map.set("zoom", 15);
                    clearPreviousMarker();
                    if (status === google.maps.GeocoderStatus.OK) {
                        var loc = results[0].geometry.location;
                        map.setCenter(loc);
                        prevMarker = new google.maps.Marker({
                            map: map,
                            position: loc
                        });
                        getcontentString(loc);
                    } else {
                        //alert("Geocode was not successful for the following reason: " + status);
                    }
                });
        },
        3000);
}

function getcontentString(pos) {
    geocoder.geocode({
            latLng: pos
        },
        function(responses) {
            var myContent;
            if (responses && responses.length > 0)
                myContent = responses[0].formatted_address;
            else
                myContent = 'Nie można ustalić dokładnego adresu';

            infowindow = new google.maps.InfoWindow({
                content: myContent
            });
            document.getElementById('RealAddress').value = myContent;
            document.getElementById('Longitude').value = pos.lat();
            document.getElementById('Latitude').value = pos.lng();
            infowindow.open(map, prevMarker);
            prevMarker.addListener('click',
                function() {
                    infowindow.open(map, prevMarker);
                });
        });
}
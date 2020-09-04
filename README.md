<img src="https://github.com/mkokabi/apiMocker/blob/master/APIMocker/icon.png?raw=true" alt="Logo" width="100px" height="100px">

# The APIMocker.Net
This application can be used to Mock an API based on the *appsettings* configuration file. While it can be used by developers it's also made portable so can be easily copied to the build server. The App has been tested on Windows, Mac and Linux.
## configuration
The configuration is only one tag *APIDetail* which contains following elements:
- Path
- Method (optional, default Get)
- QueryString (optional, default "")
- StatusCode
- ResponseBody
- ResponseBodyFile (optional)
- ResponseHeaders (optional, default no extra header)

To create an API serving /order?customerNo=12345 we need to configure as:

``` json
  "APIDetail": [
  {
    "Path": "/Order",
    "QueryString": "?customerNo=12345",
    "StatusCode": 200,
    "ResponseBody": "[{\"orderNumber\":\"23\",\"orderDate\":\"2020-07-01T06:57:53.917141+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"10\",\"orderDate\":\"2020-07-14T06:57:53.917435+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"7\",\"orderDate\":\"2020-07-08T06:57:53.917443+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"-9\",\"orderDate\":\"2020-07-14T06:57:53.917444+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"9\",\"orderDate\":\"2020-06-27T06:57:53.917468+10:00\",\"orderStatus\":\"Order\"}]"
  }],
```

The QueryString can be even replaced with 
```json
    "QueryString": "?customerNo={0}",
```
to support other values of customerNo.

## multiple API
If we have more than one APIDetail they should be ordered based on more specific first.
```json
  "APIDetail": [
    {
      "Path": "/Order",
      "QueryString": "?customerNo={0}&date={1}",
      "StatusCode": 200,
      "ResponseBody": "[{\"orderNumber\":\"23\",\"orderDate\":\"2020-07-01T06:57:53.917141+10:00\",\"orderStatus\":\"Order\"}]"
    },
    {
      "Path": "/Order",
      "QueryString": "?customerNo={0}",
      "StatusCode": 200,
      "ResponseBody": "[{\"orderNumber\":\"23\",\"orderDate\":\"2020-07-01T06:57:53.917141+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"10\",\"orderDate\":\"2020-07-14T06:57:53.917435+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"7\",\"orderDate\":\"2020-07-08T06:57:53.917443+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"-9\",\"orderDate\":\"2020-07-14T06:57:53.917444+10:00\",\"orderStatus\":\"Order\"},{\"orderNumber\":\"9\",\"orderDate\":\"2020-06-27T06:57:53.917468+10:00\",\"orderStatus\":\"Order\"}]"
    }
  ],
```

Then the APIMocker needs to be executed.
![Running the API mocker](https://github.com/mkokabi/apiMocker/blob/master/images/Running%20the%20APIMocker.png?raw=true)

It's how it looks:
MockAPI Welcome page
![API mocker](https://github.com/mkokabi/apiMocker/blob/master/images/MockAPI%20Welcome%20page.png?raw=true)
![API mocker](https://github.com/mkokabi/apiMocker/blob/master/images/APIMocker%20at%20work.png?raw=true)

## Post
The *Method* parameter could be used to configure the mock APIs other than GET.
```json
    {
      "Path": "/v1/Order",
      "Method": "post",
      "QueryString": "?customerNo={0}",
      "StatusCode": 200,
      "ResponseBody": "OK"
    }
```
![API mocker](https://github.com/mkokabi/apiMocker/blob/master/images/Running%20On%20Linux.png?raw=true)

## Response from external file
The response body can be in an external file so *ResponseBodyFile* tag should be used instead of *ResponseBody*.
```json
    {
      "Path": "/GetAllOrders",
      "StatusCode": 200,
      "ResponseBodyFile" : "responses/responseA.json"
    },
```

If we want to set on a port we can just pass:
```
dotnet run --urls "http://localhost:5600"
```
or
```
dotnet run --urls "http://localhost:5600;https://localhost:5601"
```

## Adding headers to the response
The response can have headers:
```json
    {
      "Path": "/v1/Auth",
      "Method": "post",
      "StatusCode": 401,
      "ResponseBody": "OK",
      "ResponseHeaders": 
        {"WWW-Authenticate": "Basic"}
    }
```

# API Consumer
An API application is included which is going to consume the Mock API. While this consumer is an API application itself but can be a Web application or a background service or anything else.

In it's configuration it has the URL which is currently pointint to the mock application
``` json
  "OrderApiUrl": "https://localhost:46609/order?customerNo=12345",
```

The code is simply getting this URL from the config and making a call.
``` Csharp
var client = _clientFactory.CreateClient();
var orderApiUrl = _configuration["OrderApiUrl"];

client.DefaultRequestHeaders
  .Accept
  .Add(new MediaTypeWithQualityHeaderValue("application/json"));

var request = new HttpRequestMessage(HttpMethod.Get, orderApiUrl);
var response = await client.SendAsync(request);

if (response.IsSuccessStatusCode)
{
    var responseContent = await response.Content.ReadAsStringAsync();
    var orders = JsonSerializer.Deserialize<IEnumerable<Order>>(responseContent);
    return orders;
}

```
Then we can run the API application
![Running the API consumer](https://github.com/mkokabi/apiMocker/blob/master/images/Running%20the%20API%20consumer.png?raw=true)

It's how it looks:
![API Consumer](https://github.com/mkokabi/apiMocker/blob/master/images/API%20application%20consuming%20the%20Mock.png?raw=true)


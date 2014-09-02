    ______          _                           ______         _ _   _____ _                  _ 
    | ___ \        | |                         |  ___|        (_) | /  ___| |                | |
    | |_/ /   _ ___| |__  _ __   __ _ _   _ |  |_ _ __ _   _ _| |_\_`--.| |_|__ _ _ ___ ___  | |
    |  __/ | | / __| '_ \| '_ \ / _` | | | |   |  _| '__| | | | | __|`--. \ __/ _` | '_ \ / _` |
    | |  | |_| \__ \ | | | |_) | (_| | |_| |   | | | |  | |_| | | |_/\__/ / || (_| | | | | (_| |
    \_|   \__,_|___/_| |_| .__/ \__,_|\__, |   \_| |_|   \__,_|_|\__\____/ \__\__,_|_| |_|\__,_|
                         | |           __/ |                                                  
                         |_|          |___/                                                   

A .Net MVC 5 Reference Site Demonstrating the Pushpay Platform API.


Introduction
============

Welcome to the Pushpay Fruit Stand. This reference site uses the example of purchasing fruit to demonstrate how the Pushpay API processes payments directly from a website using the Pushpay platform.

To be able to use this example yourself, you will need to:

* have administrative access to a Merchant account on Pushpay or the Pushpay developer sandbox
* have the client_id and client_secret for a "client flow" OAuth2 client configured in the Pushpay developer portal
* have configured the callback URL in the "Anticpated Payment Whitelist" for the merchant - for this demonstration it will normally need to be http://localhost:3887/home/paymentcomplete
* have the ID or handle of the merchant you want to create payments for.

(If you don’t have access to these items, refer to the Hosted Site section further below.)

With the above details at hand, open the web.config file and update the following section with the details of the server you want to create anticipated payments on:


		<add key="PushpayAPIBaseUrl" value="https://qa-api.testpushpay.com/v1/" />
		<add key="PushpayClientID" value="enter-client-id-here" />
		<add key="PushpayClientSecret" value="enter-client-secret-here" />
		<add key="OAuth2AuthorizeEndpoint" value="https://qa-auth.testpushpay.com/pushpay/oauth/authorize" />
		<add key="OAuth2TokenEndpoint" value="https://qa-auth.testpushpay.com/pushpay/oauth/token" />
		<add key="MerchantKey" value="ABCyMzp3NWFCNDlSRGNub0tvUkpBbnpBVnh1Y0FOXYZ"/>
		
		<add key="TaxPercentage" value="15"/>
		<add key="APIDocumentationUrl" value="https://qa-pushpayio.testpushpay.com/docs"/>
		<add key="SourceCodeUrl" value=""/>
		
Once configured, start the website. You should be able to add one or more fruit to your basket and then proceed to complete payment via Pushpay.

Hosted Site
===========

If you currently don't have access to either a Merchant account on Pushpay or API access, then fear not - you can still purchase your delicious virtual fruit via our hosted version of this reference site at:

[fruitstand.pushpay.io](http://fruitstand.pushpay.io/)

Don't forget to check out the Developer Console (below) to see the requests and responses being made.

Requests/Responses
==================

The Developer Console shows you exactly how the Pushpay API is operating by showing all requests and responses as they happen.

From the Fruit Stand UI, go to the top right-hand side of the screen. Click the "Developer Console" link to open the Developer Console tab on the right-hand side of the screen.

Once opened, there is the option of clicking the "New window" button to open the Developer Console in a new window, if preferred.

As you now make purchases and complete payment, you will see request/response pairs popping into the Developer Console window.

From here you can see:

* URL of the request and HTTP Method used
* contents of the request/response JSON as a collapsible tree
* the raw request or response body, which you can cut/paste from.

Anticipated Payment Flow
========================

Under the hood, the fruit stand reference site is using the "Anticipated Payment API" in Pushpay - this allows you to create a payment that you "anticipate" a user will complete (for example, completing the purchase of items in a shopping cart).

The Anticipated Payment API makes it possible to specify:

* payment amount
* who is paying (for example, email address, mobile number or existing pushpay user ID)
* any reference fields that should be set
* any fields that are read-only (for example, amount or specific reference fields).
* an internal reference for the payment, which you can use for relating the payment back to orders in a 3rd party system.

Anticipated Payments go through a set of state changes as users complete payment:


**Unassociated => Processing => Failed or Completed**

or

**Unassociated => UserCancelled**

Each of these statuses has a specific meaning:

* Unassociated = Anticipated payment has yet to be associated with a payment in the Pushpay platform.
* Processing = Anticipated payment is associated with a payment in the Pushpay platform, but that payment has yet to succeed or fail. This can indicate that the user is still entering data, or it could be a payment abandoned by the user which has yet to expire (for example, they closed their browser without completing payment).
* Failed = The payment failed - either because it was declined and the customer didn't remedy the problem, or the payment expired due to inaction.
* Completed = The payment was successful. This means the merchant has been paid.
* UserCancelled = The user explicitly clicked the "Cancel" button while on a payment screen in Pushpay, which has cancelled the anticipated payment.

Architecture
============

This reference site is built using:

* ASP.Net MVC 5
* JSON.Net (for JSON serialization/deserialization of objects)
* Thinktecture.IdentityModel.Client library (to handle OAuth2 authentication)
* SignalR (to handle pushing updates to the Developer Console as requests/responses to the API are made/received).

The Pushpay API does not currently use a database. Instead, it operates by storing order information in the session. For a real payment/cart solution, you would need to store order information in a persistent store to be able to handle processing the order once payment has been confirmed via the Anticipated Payment Status API.

Feedback
========

We love feedback on our developer resources. If you have any issues or suggestions, please record them on the github repository or contact us on twitter: [@pushpaytech](https://twitter.com/pushpaytech).

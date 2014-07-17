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

Welcome the Pushpay Fruistand - this referece site provides a demonstration of buying fruit where payment can be made via the Pushpay API, using the Pushpay platform.

To be able to use this example yourself you will need to:

* Have administrative access to a Merchant acount on puhspay or the pushpay developer sandbox.
* Have the client_id and client_secret for a "client flow" OAuth2 client configured in the Pushpay developer portal.
* Have configured the callback URL in the "Anticpated Payment Whitelist" for the merchant - for this demo it will normally need to be http://localhost:3887/home/paymentcomplete
* ID or handle of the merchant you want to create payments for.

With those details at hand you must open the web.config file and update the following section with the details of the server you want to create anticipated payments on:

		<add key="PushpayAPIBaseUrl" value="https://qa-api.testpushpay.com/v1/" />
		<add key="PushpayClientID" value="enter-client-id-here" />
		<add key="PushpayClientSecret" value="enter-client-secret-here" />
		<add key="OAuth2AuthorizeEndpoint" value="https://qa-auth.testpushpay.com/pushpay/oauth/authorize" />
		<add key="OAuth2TokenEndpoint" value="https://qa-auth.testpushpay.com/pushpay/oauth/token" />
		<add key="MerchantID" value="203328"/>
		
		<add key="TaxPercentage" value="15"/>
		<add key="APIDocumentationUrl" value="https://qa-pushpayio.testpushpay.com/docs"/>
		<add key="SourceCodeUrl" value=""/>
		
Once configured, start the website and you should be able to proceed with adding 1 or more fruit to your basket and then proceeding to complete payment via pushpay.

Hosted Site
===========

If you currently don't have access to either a Merchant account on pushpay, or API access, then fear not - you can still purchase your delicious virtual fruit via our hosted version of this reference site at:

[fruitstand.pushpay.io](http://fruitstand.pushpay.io/)

Don't forget to checkout the developer console (below) to see the requests and responses being made.

Requests/Responses
==================

If you look on the top right-hand side of the fruistand UI you will see a "developer console" link - by clicking this, you will open the developer console tab on the right-hand side of the screen.

If you then click the "New window" button, the developer console will open in a new window.

As you now make purchases and complete payment you will see request/response pairs popping into the developer console window.

From here you can see:

* URL of the request and HTTP Method used.
* Contents of the request/response JSON as a collapsible tree.
* Method for viewing the raw request or response body, which you can cut/paste from.

Anticipated Payment Flow
========================

Under the hood the fruitstand reference site is using the "Anticipated Payment API" in pushpay - this allows you to create a payment that you "anticipate" a user will complete.

The Anticipated Payment API makes it possible to specify:

* Amount
* Who is paying (email, mobile number or existing pushpay user ID)
* Any reference fields that should be set
* Control which fields are read-only e.g. amount, or specific reference fields.
* Provide a internal reference for the payment, which you can use for relating the payment back to orders in a 3rd party system.

Anticipated Payments go through a set of state changes as users complete payment:


**Unassociated => Processing => Failed or Completed**

or

**Unassociated => UserCancelled**

With each status having a specific meaning:

* Unassociated = Anticipated payment has yet to be associated with a payment in the pushpay platform.
* Processing = Anticipated payment is associated with a payment in the pushpay platform, but that payment has yet to suceed or fail.  This can indicate the user is still filling in fields, or it could be a payment abandoned by the user which has yet to expire (e.g. they closed their browser without completing payment).
* Failed = The payment failed - either because it was declined and the customer didn't remedy the problem, or the payment expired due to inaction.
* Completed = The payment was successfully made.   This means the merchant has been paid.
* UserCancelled = The user explicitly clicked the "Cancel" button while on a payment screen in pushpay, thus cancelling the anticipated payment.

Architecture
============

This reference site is built using:

* ASP.Net MVC 5
* JSON.Net (for JSON serialization/deserialization of objects)
* Thinktecture.IdentityModel.Client library (to handle OAuth2 authentication)
* SignalR (to handle pushing updates to the developer console as requests/responses to the API are made/recieved).

It does not use a database currently, instead just storing order information in the session.  For a real payment/cart solution you would need to store order information in a persistent store, so you can handle processing the order once payment has been confirmed via the Anticipated Payment Status API.

Feedback
========

We love feedback on our developer resources - if you have issues or suggestions, please add them issues on the github repository or contact us on twitter: [@pushpaytech](https://twitter.com/pushpaytech).
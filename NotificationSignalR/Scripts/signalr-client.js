"use strict";

const notyf = new Notyf({
    duration: 1000000,
    ripple: true,
    types: [
        {
            type: "info",
            background: "#010101",
            icon: false,
            dismissible: true,
        },
    ],
});

function handleNotificationClick(user, message) {
    alert(`User: ${user}, Message: ${message}`);
}

// Initialize the SignalR connection
const connection = $.hubConnection("/audit-notifications");
const notificationHub = connection.createHubProxy("notificationHub");

// Event handler for receiving notifications
notificationHub.on("receiveNotification", (user, message) => {
    const currentUser = sessionStorage.getItem("UserName");

    // Show toast notification for other users' messages
    if (currentUser !== user) {
        const notification = notyf.open({
            type: "info",
            message: `<b>${user}</b>: ${message}`,
       });

        notification.on("click", ({ target, event }) => {
            handleNotificationClick(user, me);
        });
        //showToast(`${user} says: ${message}`);
    }

    // Append the message to the message list
    appendMessageToList(user, message);
});

function generateGuid() {
    return 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, (char) => {
        const random = (Math.random() * 16) | 0; // Generate a random number between 0 and 15
        const value = char === 'x' ? random : (random & 0x3) | 0x8; // Adjust for RFC 4122 version 4 UUID
        return value.toString(16); // Convert to hexadecimal
    });
}


// Start the SignalR connection when the document is ready
$(document).ready(() => {
    // Prompt the user for their name
    const uName = prompt('Please enter your name:');
    const userId = generateGuid();

    // Check if a valid name was entered
    if (uName && uName.trim() !== "") {
        sessionStorage.setItem("UserName", uName.trim());
        sessionStorage.setItem("LoggedInUserId", userId);
    } else {
        console.log("No name entered or input was empty.");
    }

    connection
        .start()
        .done(() => {
            console.log("Connected to SignalR hub");
            console.log("Connected with transport:", connection.transport.name);
            registerUser(userId, uName);
            enableSendButton();
        })
        .fail((err) => {
            console.error("SignalR connection failed:", err);
        });
});

// Handle the send button click event
document.getElementById("sendButton").addEventListener("click", (event) => {
    event.preventDefault(); // Prevent form submission if part of a form
    sendMessage();
});

/**
 * Sends the user's message to the server.
 */
function sendMessage() {
    const user = sessionStorage.getItem("UserName") || "Anonymous";
    const message = document.getElementById("messageInput").value.trim();
    const sendToUserId = $("#userId").val();

    if (!message) {
        alert("Message cannot be empty.");
        return;
    }

    $.ajax({
        url: "/Home/NotifyClients",
        type: "POST",
        data: { user, message, sendToUserId },
        success: (response) => {
            response.success
                ? console.log(response.message)
                : console.error(response.message);
        },
        error: (xhr, status, error) => {
            console.error("Error:", error);
        },
    });


    //notificationHub.invoke("SendNotification", user, message)
    //    .then(() => {
    //        console.log("Notification sent successfully.");
    //    })
    //    .catch((err) => {
    //        console.error("Error sending notification:", err);
    //    });

}

/**
 * Appends a new message to the message list.
 * @param {string} user - The username of the sender.
 * @param {string} message - The message content.
 */
function appendMessageToList(user, message) {
    const li = document.createElement("li");
    li.textContent = `${user} says: ${message}`;
    document.getElementById("messagesList").appendChild(li);
}

/**
 * Enables the send button.
 */
function enableSendButton() {
    document.getElementById("sendButton").disabled = false;
}

/**
 * Register user on signalR
 */
function registerUser(userId, uName) {
    notificationHub.invoke("registerUser", userId)
        .then(() => {
            $("#loggedInUserId").text(userId);
            $("#loggedInUserName").text(uName);
            console.log("User registered successfully.");
        })
        .catch((err) => {
            console.error("Error while registering user:", err);
        });
}





import * as signalR from "@aspnet/signalr";
import commonHelper from "../modules/common-helper";
import {IChatContact} from "../models/chat.model";
import {LogLevel} from "@aspnet/signalr";

export default class Chat {

    private chatConnection = new signalR.HubConnectionBuilder()
        .configureLogging(LogLevel.Error)
        .withUrl("/chathub")
        .build();

    private $chatBox = $(".chat-box");
    private $chatContact = $(".chat-contact");
    private $chatloadMore = $(".chat-loadMore");
    private $chatInfo = $(".chat-box__previous-chat-info");

    private pageSize: number = 10;

    private pageIndex: number = 1;

    constructor() {

        if (window.location.pathname == "/chat") {
            this.getChatContacts();
        }

        this.setup();

        this.events();

    }

    private setup() {


        this.chatConnection.onclose = e => console.log("connection is closed");


        this.chatConnection.on("messageSeen", (messageId: number) => {
            const $messageSeen = $(`.message-seen-${messageId}`);
            $messageSeen.removeClass('d-none');
            this.scrollChatDown();
            console.log($messageSeen)

        });


        this.chatConnection.on("addChatMessage", (message: string, messageId: number, chatId: string) => {


            let $chatBody = this.$chatBox.find(".chat-box__body");
            let $chatContact = $(`.chat-contact__item[data-chat-id=${chatId}]`);

            //recipient chat box
            if (chatId != null) {

                let $chatBox = $(`.chat-box[data-chat-id = ${chatId}]`);

                //check whether the chat window is created or active
                if ($chatBox.length) {

                    $chatBody.append(message);

                    commonHelper.tooltip.reInitialize();

                    commonHelper.timeago.reInitialize();

                    this.markAsRead(chatId, messageId).then();


                } else {
                    //Append new message flag to the contact
                    if ($chatContact.length)
                        $chatContact.addClass("chat-contact__new-message");

                }
            }
            // sender chat box
            else {

                $chatBody.append(message);

                commonHelper.tooltip.reInitialize();

                commonHelper.timeago.reInitialize();

                this.scrollChatDown();

            }

        });


        this.chatConnection.on("onlineContact", (contactId) => {

            let $contactStatus = $(`.chat-contact__item[data-chat-id=${contactId}]`).find(".chat-contact__status");

            $contactStatus.removeClass("chat-contact__status--offline");
            $contactStatus.addClass("chat-contact__status--online");

        });

        this.chatConnection.on("offlineContact", (contactId) => {

            let $contactStatus = $(`.chat-contact__item[data-chat-id=${contactId}]`).find(".chat-contact__status");

            $contactStatus.addClass("chat-contact__status--offline");
            $contactStatus.removeClass("chat-contact__status--online");

        });

        this.startConnection();

    }

    private startConnection() {
        this.chatConnection.start().catch(err => console.log(err));
    }


    private events() {

        $('body').on("keyup", ".emojionearea-editor", this.chatInputKeyUpHandler);

        this.$chatContact.find(".chat-contact__input").on("keyup", this.contactSearchInputKeyUpHandler);

        this.$chatContact.on("click", ".chat-contact__name", this.createChatSession);

        this.$chatBox.find(".chat-box__body").on("scroll", this.chatBodyScrollHandler);

    }

    private chatBodyScrollHandler = () => {

        if (this.$chatBox.find(".chat-box__body").scrollTop() === 0) {
            this.pageIndex++;
            this.loadPreviousChat();
        }
    };

    private chatInputKeyUpHandler = (e: any) => {

        if (e.keyCode === 13) {

            let message: string = $(e.target).html();
            if (message.length >= 1) {

                let chatId = $(".chat-box").attr("data-chat-id");

                this.sendMessage(message, chatId);

                $(e.target).html("");
            }
        }
    };

    private contactSearchInputKeyUpHandler = (e: any) => {

        let input: any = $(e.target).val();

        this.searchContact(input);
    };

    private createChatSession = (e: any) => {
        let $chatContactNameElem = $(e.target);

        let $chatContactListItem = $chatContactNameElem.parent().parent();

        let chatId = $chatContactListItem.data("chat-id");

        const $chatSessionExist = $(`.chat-box[data-chat-id=${chatId}]`);

        if ($chatSessionExist.length) {
            return;
        }

        //reset the ff
        this.pageIndex = 1;
        this.$chatloadMore.text("");
        this.$chatInfo.hide();
      

        this.$chatBox.attr("data-chat-id", chatId);

        let chatContactName = $chatContactNameElem.html();

        $chatContactListItem.removeClass("chat-contact__new-message");


        this.$chatBox.find(".chat-box__wrapper").removeClass("d-none");

        this.$chatBox.find(".chat-box__recipient span").html(chatContactName);

        this.getChatHistory(chatId);
    };


    private getChatHistory(recipientId: string) {

        //Mark the messages as read before retrieving the messages
        this.markAsRead(recipientId).then(() => {
            this.chatConnection.invoke("getChatHistory", recipientId, 1, this.pageSize).then((data) => {


                let $chatBody = $(".chat-box__body");

                let $chatLoadMore = $(".chat-loadMore");

                if (data.item2) {
                    $chatLoadMore.text(data.item2);
                    this.$chatInfo.show();
                } else {
                    $chatLoadMore.text("");
                }

 
                $chatBody.html("");

                $chatBody.append(data.item1);

                commonHelper.tooltip.reInitialize();

                commonHelper.timeago.reInitialize();

                commonHelper.emoji.init();


               let $chatInput =  $(".emojionearea-editor");
                //clear the input field
                $chatInput.html("");
                $chatInput.focus();
                
                this.scrollChatDown();


            }, err => console.error(err));
        }, err => console.error(err));

    }


    private sendMessage(message: string, recipientId: string) {

        //Emoji plugin adds extra html to message which is not need
        //Work around is to replace it with nothing
        message = message.replace("<div><br></div>","");
        
        this.chatConnection.invoke("sendMessage", {
            recipientId: recipientId,
            content: message
        })
            .catch(err => console.log(err));
     
    }

    private getChatContacts() {

        let $chatContact = $(".chat-contact__list");

        $.post("/chat/getchatcontact", commonHelper.antiForgeryToken.add({}), (data: IChatContact[]) => {


            if (data.length) {

                this.addContactList(data);

            } else {

                let li = `<li class="chat-contact__item">
                            <h5 class="p-3">You have no friends.</h5>                                 
                        </li>`;

                $chatContact.append(li);
            }
        });

        $('.chat-contact__body').slimScroll({
            height: "50rem"
        });
    }

    private searchContact(name: string) {

        let $chatContact = $(".chat-contact__list");

        $chatContact.html("");

        $.post("/chat/getchatcontact", commonHelper.antiForgeryToken.add({
            name: name
        }), (data: IChatContact[]) => {

            if (data.length) {

                this.addContactList(data);

            } else {

                let li = `<li class="chat-contact__item">
                             <h5 class="p-3">No result was found.</h5>                                 
                        </li>`;

                $chatContact.append(li);
            }
        });
    }

    private loadPreviousChat() {


        if (this.$chatloadMore.text()) {
            let $chatBody = this.$chatBox.find(".chat-box__body");

            let recipientId = $(".chat-box").attr("data-chat-id");

            let $chatSpinner = this.$chatBox.find(".chat-box__spinner");

            $chatSpinner.show();

            this.chatConnection.invoke("getChatHistory", recipientId, this.pageIndex, this.pageSize)
                .then(data => {

                    $chatSpinner.hide();


                    $chatBody.prepend(data.item1);

                    if (data.item2) {
                        this.$chatloadMore.text(data.item2);
                        $chatBody[0].scrollTop = 100;
                        this.$chatInfo.show();

                    } else {

                        this.$chatloadMore.text("");
                        this.$chatInfo.hide();

                    }

                    commonHelper.timeago.reInitialize();

                }, err => console.error(err));
        }


    }

    private addContactList(data: Array<IChatContact>) {

        let $chatContact = $(".chat-contact__list");

        data.forEach(contact => {

            let li = `<li class="chat-contact__item"  data-chat-id="${contact.userId}">
                            <img class="chat-contact__picture" src="${contact.profilePicture}"/>
                            <a href="#" class="chat-contact__link">
                                <span class="chat-contact__name">${contact.fullName}</span>
                            </a>
                                 
                            <div class="chat-contact__status chat-contact__status--offline"></div>
                            <i class="fa fa-envelope chat-contact__icon"></i>
                        </li>`;

            $chatContact.append(li);
        });
    }

    private scrollChatDown() {

        let $chatBody = this.$chatBox.find(".chat-box__body");


        $chatBody.scrollTop($chatBody[0].scrollHeight);
        $chatBody.slimScroll({
            height: "49.5rem",
            start: 'bottom'
        });
    }

    private markAsRead(recipientId: string, messageId?: number) {

        return this.chatConnection.invoke("markMessagesAsRead", recipientId, messageId);
    }
}


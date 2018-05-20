import * as $ from "jquery";
import * as signalR from "@aspnet/signalr";
import commonHelper from "../modules/common-helper"; 

export default class Chat {

    private chatConnection = new signalR.HubConnectionBuilder().withUrl("/chathub").build();

    private $chatBox = $(".chat-box");
    private $chatContact = $(".chat-contact");
    private $chatloadMore = $(".chat-loadMore");
    private $chatInfo = $(".chat-box__previous-chat-info");

    private pageSize: number = 10;

    private pageIndex: number = 1;

    constructor() {
        
        this.getChatContacts();

        this.setup();

        this.events();
    }

    private setup() {

        this.chatConnection.onclose = e => console.log("connection is closed");

        this.chatConnection.on("addChatMessage", (message: string, senderId: string) => {
            let $chatBody = this.$chatBox.find(".chat-box__body");

            let $isTypingDiv = this.$chatBox.find(".chat-box__body .is-typing");
            $isTypingDiv.remove();
            
            if (senderId != null) {

                let $chatBox = $(`.chat-box[data-chat-id = ${senderId}]`);

                if ($chatBox.length) {

                    $chatBody.append(message);

                    commonHelper.addTimeago();

                    this.scrollChatDown();

                } else {

                    let chatContact = $(`.chat-contact__item[data-chat-id=${senderId}]`);

                    if (chatContact.length)
                        chatContact.addClass("chat-contact__new-message");

                }

            }
            else {
                $chatBody.append(message);

                commonHelper.addTimeago();

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

        this.chatConnection.on("typingNewMessage", (senderId) => {
    
            let recipientContactName = $(`[data-chat-id="${senderId}"] .chat-contact__name`).text();

            let div = ` <div class="is-typing">
            <p class=""><strong>${recipientContactName}</strong> is typing...</p>
            </div>`;

            console.log(senderId);


            if (this.$chatBox.find(".chat-box__body .is-typing").length === 0) {
                this.$chatBox.find(".chat-box__body").append(div);
            }

        });

        this.startConnection();

    }

    private startConnection() {
        this.chatConnection.start().catch(err => console.log(err));
    }


    private events() {

        this.$chatBox.find(".chat-box__input").on("keyup", this.chatInputKeyUpHandler.bind(this));


        this.$chatBox.find(".chat-box__input").on("keyup", this.isUserTyping.bind(this));

        this.$chatContact.find(".chat-contact__input").on("keyup", this.contactSearchInputKeyUpHandler.bind(this));

        this.$chatContact.on("click", ".chat-contact__name", this.createChat.bind(this));

        this.$chatBox.find(".chat-box__body").on("scroll", this.chatBodyScrollHandler.bind(this));

    }

    private chatBodyScrollHandler() {

        if (this.$chatBox.find(".chat-box__body").scrollTop() === 0) {
            this.pageIndex++;
            this.loadPreviousChat();
        }
    }

    private isUserTyping() {
        let recipientId = $(".chat-box").attr("data-chat-id");

        this.chatConnection.invoke("isTyping", recipientId);
    }

    private chatInputKeyUpHandler(e: any) {

        if (e.keyCode === 13) {

            let message:any = $(e.target).val();

            let chatId = $(".chat-box").attr("data-chat-id");

            this.sendMessage(message, chatId);

            $(e.target).val("");
        }
    }

    private contactSearchInputKeyUpHandler(e: any) {

        let input: any = $(e.target).val();

        this.searchContact(input);
    }

    private getChatHistory(recipientId: string) {

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

                $chatBody.append( data.item1);

            commonHelper.addTimeago();

            this.scrollChatDown();

        }, err => console.error(err));
    }

    private createChat(e: any) {

        //reset the ff
        this.pageIndex = 1;
        this.$chatloadMore.text("");
        this.$chatInfo.hide();

        let $chatContactNameElem = $(e.target);

        let $chatContactListItem = $chatContactNameElem.parent().parent();

        let chatId = $chatContactListItem.data("chat-id");

        this.$chatBox.attr("data-chat-id", chatId);

        let chatContactName = $chatContactNameElem.html();

        $chatContactListItem.removeClass("chat-contact__new-message");


        this.$chatBox.find(".chat-box__wrapper").removeClass("d-none");

        this.$chatBox.find(".chat-box__recipient span").html(chatContactName);

        this.getChatHistory(chatId);
    }

    private sendMessage(message: string, recipientId: string) {


        this.chatConnection.invoke("sendMessage", {
            recipientId: recipientId,
            content: message
        })
            .catch(err => console.log(err));

    }

    private getChatContacts() {

        let $chatContact = $(".chat-contact__list");

        $.post("/chat/getchatcontact", commonHelper.addAntiForgeryToken({}), (data: IChatContact[]) => {


            if (data.length) {

                this.addContactList(data);

            } else {

                let li = `<li class="chat-contact__item">
                            <h5 class="p-3">You have no friends.</h5>                                 
                        </li>`;

                $chatContact.append(li);
            }
        });
    }

    private searchContact(name: string) {

        let $chatContact = $(".chat-contact__list");

        $chatContact.html("");

        $.post("/chat/getchatcontact", commonHelper.addAntiForgeryToken({
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

                    commonHelper.addTimeago();

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
    }
}

interface IChatContact {
    userId: string;
    fullName: String;
    profilePicture: String;
}
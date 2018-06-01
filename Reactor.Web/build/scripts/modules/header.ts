import * as signalR from "@aspnet/signalr";
import commonHelper from "./common-helper";
import {NotificationTemplateType} from "../models/notification.model";

export default class Header {

    private readonly notificationConnection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();

    private readonly $miniNotificationContainer = $('.miniNotificationContainer');

    private readonly $notificationBadge = $('.notification-badge');

    private readonly notificationsUrl = "/notifications";

    private miniNotificationPageIndex: number = 1;

    constructor() {

        this.events();

        this.setup();

    }

    private events() {

        const $li = $('li.nav-item');

        $li.on('shown.bs.dropdown', (e) => {
            const $elem = $(e.target);
            $elem.find('i').addClass('text-white');

        });

        $li.on('hidden.bs.dropdown', (e) => {
            const $elem = $(e.target);
            $elem.find('i').removeClass('text-white');
        });


        $("#notifications").on('shown.bs.dropdown', this.markAllNotificationsAsRead);

        this.$miniNotificationContainer.on('scroll', this.miniNotificationScrollHandler);

        $("#logOut").on('click', this.logout);


    }

    private setup() {

        this.notificationConnection.on("notify", (notification, totalNotifications) => {

            if (this.$notificationBadge.hasClass('d-none'))
                this.$notificationBadge.removeClass('d-none');

            const $notificationDrodownLi = $('#notifications .dropdown__item');

            if ($notificationDrodownLi.length == 1) {
                const $noMiniNotification = $('.noMiniNotification');
                $noMiniNotification.remove();
            }


            this.$notificationBadge.text(totalNotifications);

            this.$miniNotificationContainer.prepend(notification);

            commonHelper.timeago.reInitialize();

            this.$miniNotificationContainer.scrollTop(0);


        });

        this.notificationConnection.start().catch(err => console.log(err));

        this.notificationConnection.onclose = err => console.log(err);
    }


    private miniNotificationScrollHandler = () => {
        const loadMore = $(".miniNotificationLoadMore").text();
        const scrollTop = this.$miniNotificationContainer.scrollTop();
        const innerHeight = this.$miniNotificationContainer.innerHeight();
        const scrollHeight = this.$miniNotificationContainer[0].scrollHeight;
        if (scrollTop + innerHeight >= scrollHeight && loadMore) {
            this.miniNotificationPageIndex++;
            this.loadMoreNotifications(NotificationTemplateType.Mini, this.miniNotificationPageIndex);
        }

    };

    private loadMoreNotifications(type: NotificationTemplateType, pageIndex: number) {
        $.ajax({
            method: "POST",
            url: `${this.notificationsUrl}/loadnotifications`,
            data: commonHelper.antiForgeryToken.add({
                pageIndex: pageIndex,
                pageSize: 10,
                type: type
            }),
            success: data => {

                if (data.type == NotificationTemplateType.Mini) {
                    this.$miniNotificationContainer.append(data.notifications);
                    commonHelper.timeago.reInitialize();

                    if (!data.loadMore) {
                        $(".miniNotificationLoadMore").html("");

                        let li = `<li class="dropdown__item text-center noMiniNotification"><span style="font-size: 1.3rem">No more notifications</span></li>`;

                        this.$miniNotificationContainer.append(li);
                    }
                }
            },
            error: () => {
                alert("An error occurred while loading the notifications. Please reload the browser.")
            }

        });

    }

    private markAllNotificationsAsRead = () => {
        $.post(`${this.notificationsUrl}/markallasread`, commonHelper.antiForgeryToken.add({}), () => {
            this.$notificationBadge.addClass('d-none');
        });
    };

    private logout = () => {
        let form = `<form action="/account/logout" method="post"><input name="__RequestVerificationToken" type="hidden" value="${commonHelper.antiForgeryToken.token()}" /></form>`;
        $(form).appendTo(document.body).submit();
        
    }

}
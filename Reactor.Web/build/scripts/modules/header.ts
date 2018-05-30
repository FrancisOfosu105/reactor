import * as signalR from "@aspnet/signalr";
import * as $ from "jquery";
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


        $("#notifications").on('show.bs.dropdown', this.markAllNotificationsAsRead);

        this.$miniNotificationContainer.on('scroll', this.miniNotificationScrollHandler);

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
            data: commonHelper.addAntiForgeryToken({
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

                        let li = `<li class="dropdown__item text-center noMiniNotification"><span style="font-size: 1.6rem">No more notifications</span>                                         </li>`;

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
        $.post(`${this.notificationsUrl}/markallasread`, commonHelper.addAntiForgeryToken({}), () => {
            this.$notificationBadge.addClass('d-none');
        });
    }

}
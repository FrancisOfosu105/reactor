import * as signalR from "@aspnet/signalr";
import * as $ from "jquery";
import commonHelper from "./common-helper";
import {NotificationTemplateType} from "../models/notification.model";

export default class Header {

    private readonly notificationConnection = new signalR.HubConnectionBuilder().withUrl("/notificationHub").build();
    private readonly $miniNotificationContainer = $('#miniNotificationContainer');
    private miniNotificationPageIndex: number = 1;

    constructor() {

        this.setup();

        this.events();
    }

    private events() {

        this.$miniNotificationContainer.on('scroll', this.miniNotificationScrollHandler);

    }

    private setup() {

        this.notificationConnection.on("notify", (notifications) => {
            const $oldNotifications = $("li#notifications");
            if ($oldNotifications.length) {
                $oldNotifications.remove();
                $(".navbar-nav").prepend(notifications);
            }
        });

        this.notificationConnection.start().catch(err => console.log(err));
    }


    private miniNotificationScrollHandler = () => {
        const loadMore = $("#miniNotificationLoadMore").text();
        if (this.$miniNotificationContainer.scrollTop() >= 390) {
            if (loadMore)
            {
                this.miniNotificationPageIndex++;
                this.loadMoreNotifications(NotificationTemplateType.Mini, this.miniNotificationPageIndex);
            }
        }
    };

    private loadMoreNotifications(type: NotificationTemplateType, pageIndex: number) {
        $.ajax({
            method: "POST",
            url: "/notification/loadnotifications",
            data: commonHelper.addAntiForgeryToken({
                pageIndex: pageIndex,
                pageSize: 10,
                type: type
            }),
            success: data => {

                if (data.type == NotificationTemplateType.Mini) {
                    this.$miniNotificationContainer.append(data.notifications);

                    if (!data.loadMore) {
                        $("#miniNotificationLoadMore").html("");

                        let li = `<li class="dropdown__item text-center" id="noMiniNotifications"><span class="" style="font-size: 1.6rem">No more notifications</span>                                         </li>`;

                        this.$miniNotificationContainer.append(li);
                    }
                    commonHelper.updateTimeago();
                }
            },
            error: () => {
                alert("An error occurred while loading the notifications. Please reload the browser.")
            }

        });

    }

}
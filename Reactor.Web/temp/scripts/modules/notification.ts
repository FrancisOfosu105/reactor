import * as $ from "jquery";
import commonHelper from "../modules/common-helper";
import {NotificationTemplateType} from "../models/notification.model";

export default class Notification {
    private readonly $window = $(window);
    private readonly $document = $(document);
    private readonly $mainNotificationContainer = $('#mainNotificationContainer');
    private mainNotificationPageIndex: number = 1;

    constructor() {

        this.geMaintNotifications();

        this.events();
    }


    private events() {
        this.$window.on('scroll', this.MainNotificationScrollHandler);
    }

    private MainNotificationScrollHandler = () => {
        const $windowHeight = this.$window.height();
        const $documentHeight = this.$document.height();
        const $windowScrollTop = this.$window.scrollTop();

        if ($documentHeight - $windowHeight === $windowScrollTop) {
            const loadMore = $("#mainNotificationLoadMore").text();
            if (loadMore) {
                this.mainNotificationPageIndex++;
                this.loadMoreNotifications(NotificationTemplateType.Main, this.mainNotificationPageIndex);

            }
        }

    };

    private geMaintNotifications() {
        $.ajax({
            method: "POST",
            url: "/notification/loadnotifications",
            data: commonHelper.addAntiForgeryToken({
                pageIndex: 1,
                pageSize: 10,
                type: NotificationTemplateType.Main
            }),
            success: data => {

                this.$mainNotificationContainer.append(data.notifications);

            },
            error: () => {
                alert("An error occurred while loading the notifications. Please reload the page.")
            }

        });


    }

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

                if (data.type == NotificationTemplateType.Main) {
                    this.$mainNotificationContainer.append(data.notifications);


                    if (!data.loadMore) {
                        $("#mainNotificationLoadMore").html("");

                        const div = `   <div class="notification__wrapper"><div class="notification__content text-center"><h4 style="font-size: 1.6rem">No more notifications.</h4></div>`;

                        this.$mainNotificationContainer.append(div);

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

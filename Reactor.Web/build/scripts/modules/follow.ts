import * as $ from "jquery";
import commonHelper from "../modules/common-helper";

export default class Follow {
    private $followButton = $(".profile__follow-btn");
    private $profileCover = $(".profile__cover");
    private $baseUrl = "/follow";

    constructor() {
        this.events();
    }

    private events() {
        this.$profileCover.on("click", ".profile__follow-btn", this.toggleFollow.bind(this));
    }

    private toggleFollow() {
        let isfollowing = this.$followButton.hasClass("profile__follow--is-following");
        let followeeUserName = this.$followButton.data("followee-username");

        if (!isfollowing) {

            this.follow(followeeUserName);
        } else {
            this.$followButton.find("span").html("Follow me");

            this.unFollow(followeeUserName);
        }
    }

    private follow(followeeUserName: string) {
        if (followeeUserName) {
            $.post(`${this.$baseUrl}/followuser`, commonHelper.addAntiForgeryToken({
                followeeUserName: followeeUserName
            }), () => {
                location.reload();
            });
        }
    }

    private unFollow(followeeUserName: any) {
        if (followeeUserName) {
            $.post(`${this.$baseUrl}/unfollowuser`, commonHelper.addAntiForgeryToken({
                followeeUserName: followeeUserName
            }), () => {
                location.reload();

            });
        }
    }
}
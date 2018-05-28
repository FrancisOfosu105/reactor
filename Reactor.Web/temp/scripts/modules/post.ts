import * as $ from "jquery";
import commonHelper from "../modules/common-helper";

export default class Post {
    private postPageIndex = 1;
    private baseUrl = "/home";
    private $body = $("body");
    private $window = $(window);
    private $document = $(document);
    private $postContainer = $("#post-container");

    constructor() {

        this.getInitialPosts();
        this.events();

    }

    private events() {
        this.$window.on("scroll", this.windowScrollHandler.bind(this));

        this.$body.on("keyup", "input.post__comment-input", this.keyPressHandler.bind(this));

        this.$body.on("click", "a.post__comment-icon", this.toggleComments.bind(this));

        this.$body.on("click", "a.post__like-icon", this.toggleLike.bind(this));

        this.$body.on("click", "a.load-previous-comments-btn", this.loadPreviousComment.bind(this));

    }


    private windowScrollHandler() {
        const documentHeight = this.$document.height();
        const windowHeight = this.$window.height();
        const windowScrollTop = this.$window.scrollTop();

        if ((documentHeight - windowHeight) === windowScrollTop) {
            this.postPageIndex++;
            this.loadMorePosts(this.postPageIndex);
        }

    }

    private toggleLike(e: MouseEvent) {

        const $likeBtn = $(e.target);

        const postId = $likeBtn.data("post-id");

        let $likeCounter = $(`.like-count-${postId}`);

        let isLiked = $likeBtn.hasClass("post__like-icon--is-liked");

        if (!isLiked)
            this.likePost(postId, $likeBtn, $likeCounter);
        else
            this.unLikePost(postId, $likeBtn, $likeCounter);


        return false;

    }

    private likePost(postId: number, $likeBtn: any, $likeCounter: any) {

        $.post(`${this.baseUrl}/likepost`, commonHelper.addAntiForgeryToken({
            postId: postId
        }), data => {

            $likeBtn.addClass("post__like-icon--is-liked");

            $likeBtn.html("Liked");

            //user and more people have liked the post.
            if (data.totalLikes >= 2)
                $likeCounter.html(`(you and ${data.totalLikes} people)`);

            //only the user has liked the post
            else
                $likeCounter.html(`(you and ${data.totalLikes} person)`);

        });
    }

    private unLikePost(postId: number, $likeBtn: any, $likeCounter: any) {
        $.post(`${this.baseUrl}/unlikepost`, commonHelper.addAntiForgeryToken({
            postId: postId
        }), data => {
            $likeBtn.removeClass("post__like-icon--is-liked");

            $likeBtn.html("Like");

            $likeCounter.html(`(${data.totalLikes})`);
        });
    }

    private getInitialPosts() {


        let url = this.$postContainer.data("url");

        if (url) {
            $.ajax({
                url: url,
                method: "POST",
                data: commonHelper.addAntiForgeryToken({
                    pageIndex: this.postPageIndex

                }),
                success: data => {
                    
                    this.$postContainer.append(data.posts);

                    //Re-initialize timeago
                    commonHelper.updateTimeago();
                }

            });

        }

    }

    private loadMorePosts(pageIndex: number) {
        const $postLoadMoreElem = $("#post-loadMore");

        let url = this.$postContainer.data("url");

        if (url) {
            if ($postLoadMoreElem.html()) {
                $.ajax({
                    method: "POST",
                    url: url,
                    data: commonHelper.addAntiForgeryToken({
                        pageIndex: pageIndex
                    }),
                    success: data => {

                       this.$postContainer.append(data.posts);

                        if (!data.loadMore) {
                            $("#post-loadMore").html("");

                            const div = `<div class="post"><div class="post__content text-center"><h4 style="font-size: 1.6rem">No more posts.</h4></div></div>`;

                           this.$postContainer.append(div);

                        }

                        //Re-initialize timeago
                        commonHelper.updateTimeago();

                    },
                    error() {
                        alert("An error occurred while loading the notifications. Please reload the page.")
                    }
                });
            }

        }
    }


    //Comments system
    private keyPressHandler(e: any) {
        if (e.keyCode === 13) {
            this.createComment($(e.target));
        }
    }

    private toggleComments(e: any) {
     
        const $anchorElem = $(e.target);
     
        const $commentBox = $($anchorElem.data("comments-target"));
     
        $commentBox.toggleClass("post__comments--show");
        
        return false;
    }

    private createComment($element: any) {

        const postId = $element.data("post-id");
        const comment = $element.val();

        if (comment.length >= 1) {
            $.post(`${this.baseUrl}/addcomment`,
                commonHelper.addAntiForgeryToken({
                    postId: postId,
                    content: comment
                }),
                data => {

                    //append comment
                    const selector = $(`.post__comments-container-${data.postId}`);
                    selector.append(data.comment);

                    //Re-initialize timeago
                    commonHelper.updateTimeago();

                    //update the comment count
                    $(`.comment-count-${data.postId}`).html(`(${data.totalComments})`);

                });
        }

        $element.val("");

    }

    private loadPreviousComment(e: Event) {

        const $loadBtn = $(e.target);

        const pageIndex: any = parseInt($loadBtn.find("span").html());

        const postId = $loadBtn.data("post-id");
        const $commentContainer = $(`.post__comments-container-${postId}`);
        const $loadMore = $(`#comment-loadMore-${postId}`);

        if ($loadMore.html()) {
            $.post(`${this.baseUrl}/previouscomments`,
                commonHelper.addAntiForgeryToken({
                    postId: postId,
                    pageIndex: pageIndex
                }),
                data => {

                    $loadBtn.find("span").html(pageIndex + 1);

                    $commentContainer.prepend(data.comments);

                    //Re-initialize timeago
                    commonHelper.updateTimeago();

                    if (!data.loadMore)
                        $loadBtn.hide();
                });
        } else {
            $loadBtn.hide();

        }

        return false;
    }
}
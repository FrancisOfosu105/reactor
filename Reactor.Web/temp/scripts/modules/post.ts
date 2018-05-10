import * as $ from "jquery";
import commonHelper from "../modules/common-helper";

export default class Post {
    private postPageIndex = 1;
    private baseUrl = "/home";
    private $body = $("body");
    private $window = $(window);
    private $document = $(document);

    constructor() {

        this.getInitialPosts();
        this.events();

    }

    private events() {
        this.$window.on("scroll", this.windowScrollHander.bind(this));

        this.$body.on("keyup", "input.post__comment-input", this.keyPressHandler.bind(this));

        this.$body.on("click", "a.post__comment-icon", this.toggleComments.bind(this));

        this.$body.on("click", "a.post__like-icon", this.toggleLike.bind(this));

        this.$body.on("click", "a.load-previous-comments-btn", this.loadPreviousComment.bind(this));

    }


    private windowScrollHander() {
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

        const postId = $likeBtn.data('post-id');

        let $likeCounter = $(`.like-count-${postId}`);

        let isLiked = $likeBtn.hasClass('post__like-icon--is-liked');

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

            $likeBtn.html('Liked');

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

            $likeBtn.html('Like');

            $likeCounter.html(`(${data.totalLikes})`);
        });
    }


    private getInitialPosts() {
        const $postContainer = $("#post-container");

        $.ajax({
            url: `${this.baseUrl}/getposts`,
            method: "POST",
            data: commonHelper.addAntiForgeryToken({
                pageIndex: this.postPageIndex

            }),
            beforeSend: () => {
                $(".loader").addClass("loader--show");

            },
            success: data => {
                $(".loader").removeClass("loader--show");

                $postContainer.append(data.posts);

                //Re-initialize timeago
                commonHelper.addTimeago();
            }

        });

    }

    private loadMorePosts(pageIndex: number) {
        const $postLoadMoreElem = $("#post-loadMore");

        if ($postLoadMoreElem.html()) {
            $.ajax({
                method: "POST",
                url: `${this.baseUrl}/getposts`,
                data: commonHelper.addAntiForgeryToken({
                    pageIndex: pageIndex
                }),
                beforeSend: () => {
                    $(".loader").addClass("loader--show");
                },
                success: data => {

                    $(".loader").removeClass("loader--show");

                    $("#post-container").append(data.posts);

                    if (!data.loadMore) {
                        $("#post-loadMore").html("");
                        $(".no-posts").removeClass("d-none");
                    }

                    //Re-initialize timeago
                    commonHelper.addTimeago();

                },
                error(jqXHR, textStatus, errorThrown) {

                }
            });
        }
    }


    //Comments system
    private keyPressHandler(e: any) {
        if (e.keyCode === 13) {
            this.createComment($(e.target));
        }
    }

    private toggleComments(e: any) {
        e.preventDefault();
        const $anchorElem = $(e.target);
        const $commentBox = $($anchorElem.data("comments-target"));
        $commentBox.toggleClass("post__comments--show");
    }

    private createComment($element: any) {

        const postId = $element.data("post-id");
        const comment = $element.val();

        if (comment.length >= 1) {
            $.post(`${this.baseUrl}/addcomment`,
                commonHelper.addAntiForgeryToken({
                    postId: postId,
                    comment: comment
                }),
                data => {
                    this.appendNewComment(data);
                });
        }

        $element.val("");

    }

    private appendNewComment(data: any) {

        const template = this.prepareTemplate(data);

        //append comment
        const selector = $(`.post__comments-container-${data.postId}`);
        selector.append(template);

        //Re-initialize timeago
        commonHelper.addTimeago();

        //update the comment count
        $(`.comment-count-${data.postId}`).html(`(${data.totalComments})`);
    }

    private loadPreviousComment(e: any) {
        e.preventDefault();
        const $loadBtn = $(e.target);
        const pageIndex = $loadBtn.find("span").html();
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
                    commonHelper.addTimeago();

                    if (!data.loadMore)
                        $loadBtn.hide();
                });
        } else {
            $loadBtn.hide();

        }

    }

    private prepareTemplate(data: any) {
        return `<div class="post__comment-list-box">        
                            <div class="post__comment-author">
                                <img src="${data.profilePicture}" class="post__comment-author-photo"/>
                            </div>

                            <div class="post__comments-list">
                                <h4 class="d-inline-block">${data.fullName}:</h4>
                                
                                <p class="d-inline-block">${data.comment}</p>
        <time class="d-inline-block pull-right post__time timeago" datetime="${data.createdOn}">

                            </div>
                         </div>
                            `;
    }

}
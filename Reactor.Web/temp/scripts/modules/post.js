"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
var $ = require("jquery");
var common_helper_1 = require("../modules/common-helper");
var Post = (function () {
    function Post() {
        this.postPageIndex = 1;
        this.baseUrl = "/home";
        this.$body = $("body");
        this.$window = $(window);
        this.$document = $(document);
        this.getInitialPosts();
        this.events();
    }
    Post.prototype.events = function () {
        this.$window.on("scroll", this.windowScrollHander.bind(this));
        this.$body.on("keyup", "input.post__comment-input", this.keyPressHandler.bind(this));
        this.$body.on("click", "a.post__comment-icon", this.toggleComments.bind(this));
        this.$body.on("click", "a.post__like-icon", this.toggleLike.bind(this));
        this.$body.on("click", "a.load-previous-comments-btn", this.loadPreviousComment.bind(this));
    };
    Post.prototype.windowScrollHander = function () {
        var documentHeight = this.$document.height();
        var windowHeight = this.$window.height();
        var windowScrollTop = this.$window.scrollTop();
        if ((documentHeight - windowHeight) === windowScrollTop) {
            this.postPageIndex++;
            this.loadMorePosts(this.postPageIndex);
        }
    };
    Post.prototype.toggleLike = function (e) {
        var $likeBtn = $(e.target);
        var postId = $likeBtn.data('post-id');
        var $likeCounter = $(".like-count-" + postId);
        var isLiked = $likeBtn.hasClass('post__like-icon--is-liked');
        if (!isLiked)
            this.likePost(postId, $likeBtn, $likeCounter);
        else
            this.unLikePost(postId, $likeBtn, $likeCounter);
        return false;
    };
    Post.prototype.likePost = function (postId, $likeBtn, $likeCounter) {
        $.post(this.baseUrl + "/likepost", common_helper_1.default.addAntiForgeryToken({
            postId: postId
        }), function (data) {
            $likeBtn.addClass("post__like-icon--is-liked");
            $likeBtn.html('Liked');
            //user and more people have liked the post.
            if (data.totalLikes >= 2)
                $likeCounter.html("(you and " + data.totalLikes + " people)");
            else
                $likeCounter.html("(you and " + data.totalLikes + " person)");
        });
    };
    Post.prototype.unLikePost = function (postId, $likeBtn, $likeCounter) {
        $.post(this.baseUrl + "/unlikepost", common_helper_1.default.addAntiForgeryToken({
            postId: postId
        }), function (data) {
            $likeBtn.removeClass("post__like-icon--is-liked");
            $likeBtn.html('Like');
            $likeCounter.html("(" + data.totalLikes + ")");
        });
    };
    Post.prototype.getInitialPosts = function () {
        var $postContainer = $("#post-container");
        $.ajax({
            url: this.baseUrl + "/getposts",
            method: "POST",
            data: common_helper_1.default.addAntiForgeryToken({
                pageIndex: this.postPageIndex
            }),
            beforeSend: function () {
                $(".loader").addClass("loader--show");
            },
            success: function (data) {
                $(".loader").removeClass("loader--show");
                $postContainer.append(data.posts);
                //Re-initialize timeago
                common_helper_1.default.addTimeago();
            }
        });
    };
    Post.prototype.loadMorePosts = function (pageIndex) {
        var $postLoadMoreElem = $("#post-loadMore");
        if ($postLoadMoreElem.html()) {
            $.ajax({
                method: "POST",
                url: this.baseUrl + "/getposts",
                data: common_helper_1.default.addAntiForgeryToken({
                    pageIndex: pageIndex
                }),
                beforeSend: function () {
                    $(".loader").addClass("loader--show");
                },
                success: function (data) {
                    $(".loader").removeClass("loader--show");
                    $("#post-container").append(data.posts);
                    if (!data.loadMore) {
                        $("#post-loadMore").html("");
                        $(".no-posts").removeClass("d-none");
                    }
                    //Re-initialize timeago
                    common_helper_1.default.addTimeago();
                },
                error: function (jqXHR, textStatus, errorThrown) {
                }
            });
        }
    };
    //Comments system
    Post.prototype.keyPressHandler = function (e) {
        if (e.keyCode === 13) {
            this.createComment($(e.target));
        }
    };
    Post.prototype.toggleComments = function (e) {
        e.preventDefault();
        var $anchorElem = $(e.target);
        var $commentBox = $($anchorElem.data("comments-target"));
        $commentBox.toggleClass("post__comments--show");
    };
    Post.prototype.createComment = function ($element) {
        var _this = this;
        var postId = $element.data("post-id");
        var comment = $element.val();
        if (comment.length >= 1) {
            $.post(this.baseUrl + "/addcomment", common_helper_1.default.addAntiForgeryToken({
                postId: postId,
                comment: comment
            }), function (data) {
                _this.appendNewComment(data);
            });
        }
        $element.val("");
    };
    Post.prototype.appendNewComment = function (data) {
        var template = this.prepareTemplate(data);
        //append comment
        var selector = $(".post__comments-container-" + data.postId);
        selector.append(template);
        //Re-initialize timeago
        common_helper_1.default.addTimeago();
        //update the comment count
        $(".comment-count-" + data.postId).html("(" + data.totalComments + ")");
    };
    Post.prototype.loadPreviousComment = function (e) {
        e.preventDefault();
        var $loadBtn = $(e.target);
        var pageIndex = $loadBtn.find("span").html();
        var postId = $loadBtn.data("post-id");
        var $commentContainer = $(".post__comments-container-" + postId);
        var $loadMore = $("#comment-loadMore-" + postId);
        if ($loadMore.html()) {
            $.post(this.baseUrl + "/previouscomments", common_helper_1.default.addAntiForgeryToken({
                postId: postId,
                pageIndex: pageIndex
            }), function (data) {
                $loadBtn.find("span").html(pageIndex + 1);
                $commentContainer.prepend(data.comments);
                //Re-initialize timeago
                common_helper_1.default.addTimeago();
                if (!data.loadMore)
                    $loadBtn.hide();
            });
        }
        else {
            $loadBtn.hide();
        }
    };
    Post.prototype.prepareTemplate = function (data) {
        return "<div class=\"post__comment-list-box\">        \n                            <div class=\"post__comment-author\">\n                                <img src=\"" + data.profilePicture + "\" class=\"post__comment-author-photo\"/>\n                            </div>\n\n                            <div class=\"post__comments-list\">\n                                <h4 class=\"d-inline-block\">" + data.fullName + ":</h4>\n                                \n                                <p class=\"d-inline-block\">" + data.comment + "</p>\n        <time class=\"d-inline-block pull-right post__time timeago\" datetime=\"" + data.createdOn + "\">\n\n                            </div>\n                         </div>\n                            ";
    };
    return Post;
}());
exports.default = Post;

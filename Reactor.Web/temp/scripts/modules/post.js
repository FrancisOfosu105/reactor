import $ from 'jquery';
import commonHelper from '../modules/common-helper';

export default class Post {

    constructor() {
        this.postPageIndex = 1;
        this.baseUrl = '/home';
        this.getInitialPosts();
        this.postEvents();


    }

    postEvents() {
        $(window).on('scroll', this.windowScrollHander.bind(this));
    }


    windowScrollHander() {
        let documentHeight = $(document).height();
        let windowHeight = $(window).height();
        let windowScrollTop = $(window).scrollTop();

        if ((documentHeight - windowHeight) === windowScrollTop) {
            this.postPageIndex++;
            this.loadMorePosts(this.postPageIndex);
        }

    }


    getInitialPosts() {
        let that = this;
        let $postContainer = $('#post-container');

        $.post(`${this.baseUrl}/getposts`, commonHelper.addAntiForgeryToken({
            pageIndex: this.postPageIndex

        }), (data) => {
            $postContainer.append(data.posts);

            //Re-initialize timeago
            commonHelper.addTimeago();

            //Call the comment events
            that.commentEvents();
        });

    }

    loadMorePosts(pageIndex) {
        let that = this;
        let $postLoadMoreElem = $('#post-loadMore');

        if ($postLoadMoreElem.html()) {
            $.ajax({
                method: 'POST',
                url: `${this.baseUrl}/getposts`,
                data: commonHelper.addAntiForgeryToken({
                    pageIndex: pageIndex
                }),
                beforeSend: function () {
                    $('.post-loader').removeClass('d-none');
                },
                success: function (data) {

                    $('.post-loader').addClass('d-none');

                    $('#post-container').append(data.posts);

                    if (!data.loadMore) {
                        $('#post-loadMore').html('');
                        $('.no-posts').removeClass('d-none');
                    }

                    //Re-initialize timeago
                    commonHelper.addTimeago();

                    //Call the comment events
                    that.commentEvents();

                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
        } else {

        }
    }


    //Comments system

    commentEvents() {
        let $commentInput = $('.post__comment-input');
        let $commentIcon = $('.post__comment-icon');
        let $loadPreviousCommentBtn = $('.load-previous-comments-btn');

        $commentInput.keyup(this.keyPressHandler.bind(this));
        $commentIcon.on('click', this.toggleComments.bind(this));
        $loadPreviousCommentBtn.on('click', this.loadPreviousComment.bind(this));
    }


    keyPressHandler(e) {
        if (e.keyCode === 13) {
            this.createComment($(e.target));
        }
    }

    toggleComments(e) {
        let $anchorElem = $(e.target);
        let $commentBox = $($anchorElem.data('comments-target'));
        $commentBox.toggleClass('post__comments--show');
        return false;
    }

    createComment($element) {

        let postId = $element.data('post-id');
        let comment = $element.val();
        let that = this;

        if (comment.length >= 1) {
            $.post(`${this.baseUrl}/addcomment`, commonHelper.addAntiForgeryToken({
                postId: postId,
                comment: comment
            }), function (data) {
                that.appendNewComment(data);
            })
        }

        $element.val('');

    }

    appendNewComment(data) {

        let template = this.prepareTemplate(data);

        //append comment
        let selector = $(`.post__comments-container-${data.postId}`);
        selector.append(template);

        //Re-initialize timeago
        commonHelper.addTimeago();

        //update the comment count
        $(`.comment-count-${data.postId}`).html(`(${data.totalComments})`);
    }

    loadPreviousComment(e) {
        e.preventDefault();
        let that = this;
        let $loadBtn = $(e.target);
        let pageIndex = parseInt($loadBtn.find('span').html());
        let postId = $loadBtn.data('post-id');
        let $commentContainer = $(`.post__comments-container-${postId}`);
        let $loadMore = $(`#comment-loadMore-${postId}`);


        if ($loadMore.html()) {
            $.post(`${this.baseUrl}/previouscomments`, commonHelper.addAntiForgeryToken({
                postId: postId,
                pageIndex: pageIndex
            }), function (data) {

                $loadBtn.find('span').html(pageIndex + 1);

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

    prepareTemplate(data) {
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
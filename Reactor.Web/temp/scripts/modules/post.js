import $ from 'jquery';
import commonHelper from '../modules/common-helper';

export default class Post {

    constructor() {
        this.postPageIndex = 1;
        this.baseUrl = '/home';
        this.$body = $('body');
        this.$window = $(window);
        this.$document = $(document);
        this.getInitialPosts();
        this.events();
    }

    events() {
        this.$window.on('scroll', this.windowScrollHander.bind(this));

        this.$body.on('keyup', 'input.post__comment-input', this.keyPressHandler.bind(this));

        this.$body.on('click', 'a.post__comment-icon', this.toggleComments.bind(this));

        this.$body.on('click', 'a.load-previous-comments-btn', this.loadPreviousComment.bind(this));
    }


    windowScrollHander() {
        let documentHeight = this.$document.height();
        let windowHeight = this.$window.height();
        let windowScrollTop = this.$window.scrollTop();

        if ((documentHeight - windowHeight) === windowScrollTop) {
            this.postPageIndex++;
            this.loadMorePosts(this.postPageIndex);
        }

    }


    getInitialPosts() {
        let $postContainer = $('#post-container');
        
        $.ajax({
            url :`${this.baseUrl}/getposts`,
            method: 'POST',
            data :commonHelper.addAntiForgeryToken({
                pageIndex: this.postPageIndex

            }),
            beforeSend: ()=>{
                $('.loader').addClass('loader--show');

            },
            success: data => {
                $('.loader').removeClass('loader--show');

                $postContainer.append(data.posts);

                //Re-initialize timeago
                commonHelper.addTimeago();
            }
            
        });

    }

    loadMorePosts(pageIndex) {
                let $postLoadMoreElem = $('#post-loadMore');

        if ($postLoadMoreElem.html()) {
            $.ajax({
                method: 'POST',
                url: `${this.baseUrl}/getposts`,
                data: commonHelper.addAntiForgeryToken({
                    pageIndex: pageIndex
                }),
                beforeSend: function () {
                    $('.loader').addClass('loader--show');
                },
                success: function (data) {

                    $('.loader').removeClass('loader--show');

                    $('#post-container').append(data.posts);

                    if (!data.loadMore) {
                        $('#post-loadMore').html('');
                        $('.no-posts').removeClass('d-none');
                    }

                    //Re-initialize timeago
                    commonHelper.addTimeago();

                },
                error: function (jqXHR, textStatus, errorThrown) {

                }
            });
        } else {

        }
    }


    //Comments system
    keyPressHandler(e) {
        if (e.keyCode === 13) {
            this.createComment($(e.target));
        }
    }

    toggleComments(e) {
        e.preventDefault();
        let $anchorElem = $(e.target);
        let $commentBox = $($anchorElem.data('comments-target'));
        $commentBox.toggleClass('post__comments--show');
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
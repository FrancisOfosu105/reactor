import $ from 'jquery';
import commonHelper from '../modules/common-helper'

export default class Comment {

    constructor() {
        this.commentInput = $('.post__comment-input');
        this.commentIcon = $('.post__comment-icon');
        this.loadPreviousCommentBtn = $('.load-previous-comments-btn');
        this.events();
    }

    events() {
        this.commentInput.keyup(this.keyPressHandler.bind(this));
        this.commentIcon.on('click', this.toggleComments.bind(this));
        this.loadPreviousCommentBtn.on('click', this.loadPreviousComment.bind(this));
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
            $.post('/home/addcomment', commonHelper.addAntiForgeryToken({
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

        $.post('/home/previouscomments', commonHelper.addAntiForgeryToken({
            postId: postId,
            pageIndex: pageIndex
        }), function (data) {

            let commentsToBeAppend = [];
            data.comments.forEach(comment => {

                let template = that.prepareTemplate(comment);
                commentsToBeAppend.push(template);

            });
                      
            $loadBtn.find('span').html(pageIndex + 1);
            
            $commentContainer.prepend(commentsToBeAppend.join(''));
            commonHelper.addTimeago();

            //hide the load more button
            if (!data.loadMore) {
                $loadBtn.hide();
            } 

        });

    }

    prepareTemplate(data) {
        let template = `<div class="post__comment-list-box">        
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

        return template;
    }
}
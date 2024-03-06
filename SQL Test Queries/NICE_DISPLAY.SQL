SELECT conversationid, userid, conversationname, username
FROM user_conversation_link
LEFT JOIN conversations ON user_conversation_link.conversationid = conversations.id
LEFT JOIN users ON user_conversation_link.userid = users.id;

SELECT content as "Message", conversationname as "In", username as "SendBy"
FROM messagestable
LEFT JOIN user_conversation_link ON messagestable.conversationid = user_conversation_link.conversationid
JOIN conversations ON user_conversation_link.conversationid = conversations.id AND user_conversation_link.conversationid = messagestable.conversationid
JOIN users ON user_conversation_link.userid = users.id AND user_conversation_link.userid = messagestable.userid;
;
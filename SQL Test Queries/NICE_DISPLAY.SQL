SELECT conversationid, userid, conversationname, username
FROM user_conversation_link
LEFT JOIN conversations ON user_conversation_link.conversationid = conversations.id
LEFT JOIN users ON user_conversation_link.userid = users.id;

SELECT content as "Message", conversationname as "In", username as "By"
FROM messagestable
LEFT JOIN user_conversation_link ON messagestable.conversationid = user_conversation_link.conversationid
LEFT JOIN conversations ON user_conversation_link.conversationid = conversations.id
LEFT JOIN users ON user_conversation_link.userid = users.id;
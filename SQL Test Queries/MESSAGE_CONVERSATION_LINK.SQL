CREATE TABLE user_conversation_link(
	conversationid INT,
	userid INT
);

INSERT INTO user_conversation_link(conversationid, userid)
VALUES(1, 1);

DROP TABLE user_conversation_link;
CREATE TABLE users(
	id INT PRIMARY KEY IDENTITY(1,1),
	username VARCHAR(100),
	password VARCHAR(100)
);

INSERT INTO users(username,password)
VALUES('Levithan7','123');

DROP TABLE users;
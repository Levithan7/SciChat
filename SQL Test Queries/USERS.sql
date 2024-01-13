CREATE TABLE users(
	id INT PRIMARY KEY IDENTITY(1,1),
	username VARCHAR(100)
);

INSERT INTO users(username)
VALUES('Levithan7');

DROP TABLE users;
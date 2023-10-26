Feature: Authentication

I want to register, login or know who am i on management

Background: 
	Given I have these following users in database
	| Email          | Name  | PhoneNumber | Password                 |
	| user1@mail.com | user1 | 690000000   | User1@test-password |
	| user2@mail.com | user2 | 690000001   | User2@test-password |
	| user3@mail.com | user3 | 690000002   | User2@test-password |

@login
Scenario: Login failed when username or password invalid
	Given I have this information as connection information
	| Username | Password |
	|          |          |
	When I launch an http login request with my informations
	Then I get a status 500 response

@login
Scenario: Login failed when username or password incorrect
	Given I have this information as connection information
	| Username        | Password    |
	| user10@test.com | passwordtes |
	When I launch an http login request with my informations
	Then I get a status 200 response
	* I have following data as response data
	| IsSuccess | Message                        |
	| false     | Incorrect username or password |

@login
Scenario: Login success with valid username and password
	Given I have this information as connection information
	| Username       | Password            |
	| user1@mail.com | User1@test-password |
	When I launch an http login request with my informations
	Then I get a status 200 response
	* I have following data as response data
	| IsSuccess | Message                     |
	| true      | User logged in successfully |
	* I have following result
	| Email          | Name  | PhoneNumber | Roles   |
	| user1@mail.com | user1 | 690000000   | MANAGER |
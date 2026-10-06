drop table if exists user;
create table user (
  user_id integer primary key autoincrement,
  username string not null,
  email string not null,
  pw_hash string not null
);

drop table if exists observation;
create table observation (
  observation_id integer primary key autoincrement,
  author_id integer not null,
  text string not null,
  pub_date integer
);

drop table if exists comment;
create table comment (
  comment_id integer primary key autoincrement,
  observation_id integer not null,
  author_id integer not null,
  text string not null,
  pub_date integer
);

drop table if exists proposal;
create table proposal (
  proposal_id integer primary key autoincrement,
  observation_id integer not null,
  author_id integer not null,
  taxon_id string not null,
  pub_date integer
);
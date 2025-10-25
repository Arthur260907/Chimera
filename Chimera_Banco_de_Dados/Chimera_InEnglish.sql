Create database chimera;
use chimera;

create table streamings (
    id_streaming int primary key auto_increment,
    name_streaming varchar(255) not null,
    price float,
    url varchar(255)
);

create table users (
    id int primary key auto_increment,
    username varchar(255) not null,
    email varchar(255) not null,
    userspassword varchar(255) not null,
    date_cadastro datetime,
    perfil_pic varchar(255),
    token_recovery VARCHAR(255),
    token_expiration DATETIME
);


create table media (
    id_streaming int,
	imdb_id VARCHAR(32) PRIMARY KEY,
    title VARCHAR(255) NOT NULL,
    launch_year YEAR(4),
    rated VARCHAR(16),
    released DATE,
    runtime_minutes INT,
    genre VARCHAR(255),
    director VARCHAR(255),
    writer VARCHAR(255),
    actors VARCHAR(255),
    plot VARCHAR(255),
    language VARCHAR(255),
    country VARCHAR(255),
    awards VARCHAR(255),
    poster_url VARCHAR(1024),
    imdb_rating DECIMAL(3, 1),
    type VARCHAR(32),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table episodes (
    id int primary key auto_increment,
    imdb_id VARCHAR(32),
    episodie_number int not null,
    title varchar(255) not null,
    duration int,
    foreign key (imdb_id) references media(imdb_id)
);

create table access_history (
    id_history int primary key auto_increment,
    id_user int,
    id_conteudo int not null,
    tipo_conteudo varchar(20) not null,
    data_hora_acesso datetime,
    minutos_assistidos int,
    conclusao int,
    foreign key (id_user) references users(id)
);

create table assessment (
    id_avaliacao int primary key auto_increment,
    id_user int,
    id_content int not null,
    type_content varchar(20) not null,
    rating int,
    comentary text,
    assessment_date datetime,
    foreign key (id_user) references users(id)
);

create table watch_later_list (
    id_list int primary key auto_increment,
    id_user int,
    name_list varchar(255) not null,
    creation_date date,
    foreign key (id_user) references users(id)
);

create table content_list (
    id_list int,
    id_content int,
    type_content varchar(20) not null,
    primary key (id_list, id_content, type_content),
    foreign key (id_list) references watch_later_list(id_list)
);

create table notification (
    id_notification int primary key auto_increment,
    id_user int,
    tipo varchar(100),
    message text,
    send_date datetime,
    already_read boolean,
    foreign key (id_user) references users(id)
);

create table social_network_integration (
    id_integration int primary key auto_increment,
    id_user int,
    type_network varchar(50) not null,
    id_external varchar(255) not null,
    token_access text,
    foreign key (id_user) references users(id)
);

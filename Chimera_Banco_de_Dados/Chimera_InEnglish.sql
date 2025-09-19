create database Chimera;
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


create table movies (
    id int primary key auto_increment,
    id_streaming int,
    movie_title varchar(255) not null,
    language varchar(50),
    date_addition datetime,
    rating_imdb float,
    rating_rottentomatoes float,
    rating_metacritic float,
    rating varchar(100),
    synopsis text,
    time_watched int,
    cast text,
    director varchar(255),
    trailer varchar(255),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table series (
    id int primary key auto_increment,
    id_streaming int,
    serie_title varchar(255) not null,
    lenguage varchar(50),
    date_addition datetime,
	rating_imdb float,
    rating_rottentomatoes float,
    rating_metacritic float,
	rating varchar(100),
    synopsis text,
    time_watched int,
    cast text,
    director varchar(255),
    trailer varchar(255),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table episodes (
    id int primary key auto_increment,
    id_serie int,
    episodie_number int not null,
    title varchar(255) not null,
    duration int,
    foreign key (id_serie) references series(id)
);

create table payments (
    id_payments int primary key auto_increment,
    id_user int,
    date_payment datetime,
    value_payment float,
    invoice_due_date date,
    foreign key (id_user) references users(id)
);

create table card (
    id_card int primary key auto_increment,
    card_number varchar(255) not null,
    id_user int,
    holder_name varchar(255),
    validity date,
    cvc varchar(255),
    card_type varchar(20),
    foreign key (id_user) references users(id)
);

create table signature (
    id_signature int primary key auto_increment,
    id_user int,
    id_streaming int,
    plan_type varchar(50),
    date_inicio date,
    date_validity date,
	mensal_value float,
    signature_status varchar(20),
    foreign key (id_user) references users(id),
    foreign key (id_streaming) references streamings(id_streaming)
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

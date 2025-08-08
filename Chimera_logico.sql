create database Chimera;
use chimera;

create table streamings (
    id_streaming int primary key auto_increment,
    nome varchar(255) not null,
    preco float,
    url varchar(255)
);

create table usuarios (
    id int primary key auto_increment,
    nome varchar(255) not null,
    email varchar(255) not null,
    senha varchar(255) not null,
    data_cadastro datetime,
    foto_perfil varchar(255)
);

create table filmes (
    id int primary key auto_increment,
    id_streaming int,
    nome varchar(255) not null,
    idioma varchar(50),
    data_adicionado datetime,
    nota_imdb float,
    nota_rottentomatoes float,
    classificacao varchar(100),
    sinopse text,
    tempo_assistido int,
    elenco text,
    diretor varchar(255),
    trailer varchar(255),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table series (
    id int primary key auto_increment,
    id_streaming int,
    nome varchar(255) not null,
    idioma varchar(50),
    data_adicionado datetime,
    nota_imdb float,
    nota_rottentomatoes float,
    classificacao varchar(100),
    sinopse text,
    tempo_assistido int,
    elenco text,
    diretor varchar(255),
    trailer varchar(255),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table episodios (
    id int primary key auto_increment,
    id_serie int,
    numero_episodio int not null,
    titulo varchar(255) not null,
    duracao int,
    foreign key (id_serie) references series(id)
);

create table pagamento (
    id_pagamento int primary key auto_increment,
    id_usuario int,
    data datetime,
    valor float,
    vencimento_fatura date,
    foreign key (id_usuario) references usuarios(id)
);

create table cartao (
    id_cartao int primary key auto_increment,
    numero_cartao varchar(255) not null,
    id_usuario int,
    nome_titular varchar(255),
    validade date,
    cvc varchar(255),
    tipo varchar(20),
    foreign key (id_usuario) references usuarios(id)
);

create table assinatura (
    id_assinatura int primary key auto_increment,
    id_usuario int,
    id_streaming int,
    tipo_plano varchar(50),
    data_inicio date,
    data_vencimento date,
    valor_mensal float,
    status varchar(20),
    foreign key (id_usuario) references usuarios(id),
    foreign key (id_streaming) references streamings(id_streaming)
);

create table historico_acesso (
    id_historico int primary key auto_increment,
    id_usuario int,
    id_conteudo int not null,
    tipo_conteudo varchar(20) not null,
    data_hora_acesso datetime,
    minutos_assistidos int,
    conclusao int,
    foreign key (id_usuario) references usuarios(id)
);

create table avaliacao (
    id_avaliacao int primary key auto_increment,
    id_usuario int,
    id_conteudo int not null,
    tipo_conteudo varchar(20) not null,
    nota int,
    comentario text,
    data_avaliacao datetime,
    foreign key (id_usuario) references usuarios(id)
);

create table lista_assistir_mais_tarde (
    id_lista int primary key auto_increment,
    id_usuario int,
    nome_lista varchar(255) not null,
    data_criacao date,
    foreign key (id_usuario) references usuarios(id)
);

create table lista_conteudo (
    id_lista int,
    id_conteudo int,
    tipo_conteudo varchar(20) not null,
    primary key (id_lista, id_conteudo, tipo_conteudo),
    foreign key (id_lista) references lista_assistir_mais_tarde(id_lista)
);

create table notificacao (
    id_notificacao int primary key auto_increment,
    id_usuario int,
    tipo varchar(100),
    mensagem text,
    data_envio datetime,
    lida boolean,
    foreign key (id_usuario) references usuarios(id)
);

create table integracao_redes_sociais (
    id_integracao int primary key auto_increment,
    id_usuario int,
    tipo_rede varchar(50) not null,
    id_externo varchar(255) not null,
    token_acesso text,
    foreign key (id_usuario) references usuarios(id)
);
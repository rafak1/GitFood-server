-- This script was generated by a beta version of the ERD tool in pgAdmin 4.
-- Please log an issue at https://redmine.postgresql.org/projects/pgadmin4/issues/new if you find any bugs, including reproduction steps.
BEGIN;


CREATE TABLE IF NOT EXISTS public.barcodes
(
    key character varying COLLATE pg_catalog."default" NOT NULL,
    product_id integer,
    "user" character varying,
    CONSTRAINT "Barcode_pkey" PRIMARY KEY (key)
);

ALTER TABLE IF EXISTS public.barcodes
    ENABLE ROW LEVEL SECURITY;

CREATE TABLE IF NOT EXISTS public.products
(
    id serial NOT NULL,
    description character varying COLLATE pg_catalog."default",
    name character varying COLLATE pg_catalog."default" NOT NULL,
    CONSTRAINT "Products_pkey" PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.products_categories
(
    product_id integer NOT NULL,
    category_id integer NOT NULL,
    PRIMARY KEY (product_id, category_id)
);

CREATE TABLE IF NOT EXISTS public.categories
(
    id serial NOT NULL,
    name character varying NOT NULL,
    PRIMARY KEY (id)
);

CREATE TABLE IF NOT EXISTS public.users
(
    login character varying NOT NULL,
    password character varying NOT NULL,
    PRIMARY KEY (login)
);

CREATE TABLE IF NOT EXISTS public.fridge
(
    product_id integer NOT NULL,
    user_login character varying NOT NULL,
    id serial NOT NULL,
    PRIMARY KEY (id),
    UNIQUE (product_id, user_login)
);

CREATE TABLE IF NOT EXISTS public.fridge_units
(
    fridge_product_id integer NOT NULL,
    quantity double precision NOT NULL,
    unit character varying NOT NULL,
    PRIMARY KEY (fridge_product_id, unit)
);

ALTER TABLE IF EXISTS public.barcodes
    ADD CONSTRAINT "Barcode_ProductId_fkey" FOREIGN KEY (product_id)
    REFERENCES public.products (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.barcodes
    ADD FOREIGN KEY ("user")
    REFERENCES public.users (login) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.products_categories
    ADD FOREIGN KEY (product_id)
    REFERENCES public.products (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.products_categories
    ADD FOREIGN KEY (category_id)
    REFERENCES public.categories (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.fridge
    ADD FOREIGN KEY (product_id)
    REFERENCES public.products (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.fridge
    ADD FOREIGN KEY (user_login)
    REFERENCES public.users (login) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;


ALTER TABLE IF EXISTS public.fridge_units
    ADD FOREIGN KEY (fridge_product_id)
    REFERENCES public.fridge (id) MATCH SIMPLE
    ON UPDATE NO ACTION
    ON DELETE NO ACTION
    NOT VALID;

END;
-- migrate:up

CREATE TABLE public.events_01_restaurant (
                                          id integer NOT NULL,
                                          event text NOT NULL,
                                          published boolean NOT NULL DEFAULT false,
                                          kafkaoffset BIGINT,
                                          kafkapartition INTEGER,
                                          "timestamp" timestamp without time zone NOT NULL
);

ALTER TABLE public.events_01_restaurant ALTER COLUMN id ADD GENERATED ALWAYS AS IDENTITY (
    SEQUENCE NAME public.events_01_restaurant_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);

CREATE SEQUENCE public.snapshots_01_restaurant_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE TABLE public.snapshots_01_restaurant (
                                             id integer DEFAULT nextval('public.snapshots_01_restaurant_id_seq'::regclass) NOT NULL,
                                             snapshot text NOT NULL,
                                             event_id integer NOT NULL,
                                             "timestamp" timestamp without time zone NOT NULL
);

ALTER TABLE ONLY public.events_01_restaurant
    ADD CONSTRAINT events_restaurant_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.snapshots_01_restaurant
    ADD CONSTRAINT snapshots_restaurant_pkey PRIMARY KEY (id);

ALTER TABLE ONLY public.snapshots_01_restaurant
    ADD CONSTRAINT event_01_restaurant_fk FOREIGN KEY (event_id) REFERENCES public.events_01_restaurant(id) MATCH FULL ON DELETE CASCADE;


CREATE OR REPLACE FUNCTION insert_01_restaurant_event_and_return_id(
    IN event_in TEXT
)
RETURNS int
       
LANGUAGE plpgsql
AS $$
DECLARE
    inserted_id integer;
BEGIN
    INSERT INTO events_01_restaurant(event, timestamp)
    VALUES(event_in::text, now()) RETURNING id INTO inserted_id;
    return inserted_id;

END;
$$;

-- CREATE OR REPLACE PROCEDURE set_classic_optimistic_lock_01_restaurant() AS $$
-- BEGIN 
--     IF NOT EXISTS (SELECT 1 FROM pg_constraint WHERE conname = 'context_events_01_restaurant_context_state_id_unique') THEN
-- ALTER TABLE events_01_restaurant
--     ADD CONSTRAINT context_events_01_restaurant_context_state_id_unique UNIQUE (context_state_id);
-- END IF;
-- END;
-- $$ LANGUAGE plpgsql;

-- CREATE OR REPLACE PROCEDURE un_set_classic_optimistic_lockcontext_events_01_restaurant() AS $$
-- BEGIN
--     ALTER TABLE eventscontext_events_01_restaurant
--     DROP CONSTRAINT IF EXISTS context_eventscontext_events_01_restaurant_context_state_id_unique; 
-- END;
-- $$ LANGUAGE plpgsql;

-- migrate:down



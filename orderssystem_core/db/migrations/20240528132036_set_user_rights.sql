-- migrate:up
GRANT ALL ON TABLE public.events_01_restaurant TO safe;
GRANT ALL ON TABLE public.snapshots_01_restaurant TO safe;
GRANT ALL ON SEQUENCE public.snapshots_01_restaurant_id_seq TO safe;
GRANT ALL ON SEQUENCE public.events_01_restaurant_id_seq TO safe;
          
GRANT ALL ON TABLE public.aggregate_events_01_users TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_users_id_seq to safe;
GRANT ALL ON TABLE public.events_01_users to safe;
GRANT ALL ON TABLE public.snapshots_01_users to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_users_id_seq to safe;          

GRANT ALL ON TABLE public.aggregate_events_01_tables TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_tables_id_seq to safe;
GRANT ALL ON TABLE public.events_01_tables to safe;
GRANT ALL ON TABLE public.snapshots_01_tables to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_tables_id_seq to safe;
          
-- GRANT ALL ON TABLE public.aggregate_events_01_roles TO safe;
-- GRANT ALL ON SEQUENCE public.aggregate_events_01_roles_id_seq to safe;
-- GRANT ALL ON TABLE public.events_01_roles to safe;
-- GRANT ALL ON TABLE public.snapshots_01_roles to safe;
-- GRANT ALL ON SEQUENCE public.snapshots_01_roles_id_seq to safe;

GRANT ALL ON TABLE public.aggregate_events_01_orderItems TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_orderItems_id_seq to safe;
GRANT ALL ON TABLE public.events_01_orderItems to safe;
GRANT ALL ON TABLE public.snapshots_01_orderItems to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_orderItems_id_seq to safe;

GRANT ALL ON TABLE public.aggregate_events_01_orders TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_orders_id_seq to safe;
GRANT ALL ON TABLE public.events_01_orders to safe;
GRANT ALL ON TABLE public.snapshots_01_orders to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_orders_id_seq to safe;          

GRANT ALL ON TABLE public.aggregate_events_01_ingredients TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_ingredients_id_seq to safe;
GRANT ALL ON TABLE public.events_01_ingredients to safe;
GRANT ALL ON TABLE public.snapshots_01_ingredients to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_ingredients_id_seq to safe;
         
GRANT ALL ON TABLE public.aggregate_events_01_dishes TO safe;
GRANT ALL ON SEQUENCE public.aggregate_events_01_dishes_id_seq to safe;
GRANT ALL ON TABLE public.events_01_dishes to safe;
GRANT ALL ON TABLE public.snapshots_01_dishes to safe;
GRANT ALL ON SEQUENCE public.snapshots_01_dishes_id_seq to safe;
          
          
-- migrate:down


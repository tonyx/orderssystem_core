--
-- PostgreSQL database dump
--

-- Dumped from database version 9.6.0
-- Dumped by pg_dump version 9.6.8

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: plpgsql; Type: EXTENSION; Schema: -; Owner: 
--

CREATE EXTENSION IF NOT EXISTS plpgsql WITH SCHEMA pg_catalog;


--
-- Name: EXTENSION plpgsql; Type: COMMENT; Schema: -; Owner: 
--

COMMENT ON EXTENSION plpgsql IS 'PL/pgSQL procedural language';


SET default_tablespace = '';

SET default_with_oids = false;

--
-- Name: archivedorderslogbuffer; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.archivedorderslogbuffer (
    archivedlogbufferid integer NOT NULL,
    archivedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL
);



--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.archivedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.archivedorderslog_id_seq OWNED BY public.archivedorderslogbuffer.archivedlogbufferid;


--
-- Name: coursecategories; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.coursecategories (
    categoryid integer NOT NULL,
    name character varying(50) NOT NULL,
    visibile boolean DEFAULT true,
    abstract boolean NOT NULL
);



--
-- Name: courses; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.courses (
    courseid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    price numeric(10,2) DEFAULT 0,
    categoryid integer NOT NULL,
    visibility boolean NOT NULL
);



--
-- Name: coursedetails2; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.coursedetails2 AS
 SELECT a.courseid,
    a.name,
    a.description,
    a.price,
    c.name AS coursecategoryname,
    c.categoryid,
    a.visibility
   FROM (public.courses a
     JOIN public.coursecategories c ON ((a.categoryid = c.categoryid)))
  ORDER BY a.courseid;



--
-- Name: courses_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.courses_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: courses_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.courses_categoryid_seq OWNED BY public.coursecategories.categoryid;


--
-- Name: courses_courseid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.courses_courseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: courses_courseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.courses_courseid_seq OWNED BY public.courses.courseid;


--
-- Name: customerdata_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.customerdata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: customerdata; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.customerdata (
    customerdataid integer DEFAULT nextval('public.customerdata_id_seq'::regclass) NOT NULL,
    data character varying(4000) NOT NULL,
    name character varying(300) NOT NULL
);



--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.defaulwaiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.defaulwaiteractionablestates_seq OWNER TO "Tonyx";

--
-- Name: defaultactionablestates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.defaultactionablestates (
    defaultactionablestatesid integer DEFAULT nextval('public.defaulwaiteractionablestates_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: enablers_elablersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.enablers_elablersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: enablers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.enablers (
    enablersid integer DEFAULT nextval('public.enablers_elablersid_seq'::regclass) NOT NULL,
    roleid integer NOT NULL,
    stateid integer NOT NULL,
    categoryid integer NOT NULL
);



--
-- Name: roles; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.roles (
    roleid integer NOT NULL,
    rolename character varying(30) NOT NULL,
    comment character varying(50)
);



--
-- Name: states_stateid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.states_stateid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.states (
    stateid integer DEFAULT nextval('public.states_stateid_seq'::regclass) NOT NULL,
    isinitial boolean NOT NULL,
    isfinal boolean NOT NULL,
    statusname character varying(30) NOT NULL,
    nextstateid integer,
    isexceptional boolean NOT NULL,
    creatingingredientdecrement boolean NOT NULL
);



--
-- Name: enablersrolestatuscategories; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.enablersrolestatuscategories AS
 SELECT a.enablersid,
    c.rolename,
    d.name AS categoryname,
    e.statusname
   FROM (((public.enablers a
     JOIN public.roles c ON ((a.roleid = c.roleid)))
     JOIN public.coursecategories d ON ((a.categoryid = d.categoryid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));



--
-- Name: incredientdecrementid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.incredientdecrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: ingredient; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredient (
    ingredientid integer NOT NULL,
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL,
    allergen boolean NOT NULL,
    updateavailabilityflag boolean NOT NULL,
    availablequantity numeric(10,2),
    checkavailabilityflag boolean NOT NULL,
    unitmeasure character varying(20) NOT NULL
);



CREATE SEQUENCE public.standard_comments_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE TABLE public.standardcomments (
    standardcommentid integer NOT NULL,
    comment character varying(59) NOT NULL
);

ALTER TABLE ONLY public.standardcomments ALTER COLUMN standardcommentid SET DEFAULT nextval('public.standard_comments_seq');


ALTER TABLE ONLY public.standardcomments
    ADD CONSTRAINT comment_uniquename UNIQUE (comment);


ALTER TABLE ONLY public.standardcomments
    ADD CONSTRAINT standardcomment_pkey PRIMARY KEY (standardcommentid);



CREATE SEQUENCE public.comments_for_course_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


CREATE TABLE public.commentsforcourse (
    commentsforcourseid integer NOT NULL,
    courseid integer NOT NULL,
    standardcommentid integer NOT NULL
);

ALTER TABLE ONLY public.commentsforcourse ALTER COLUMN commentsforcourseid SET DEFAULT nextval('public.comments_for_course_seq');
ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT commentsforcourse_pkey PRIMARY KEY (commentsforcourseid);
ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT coursecommentid_pair UNIQUE (courseid,standardcommentid);

ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT commentsforcourse_fk FOREIGN KEY (standardcommentid) REFERENCES public.standardcomments(standardcommentid) MATCH FULL ON DELETE CASCADE;

--
-- Name: ingredientcategory; Type: TABLE; Schema: public; Owner: Tonyx
-- 
CREATE VIEW public.commentsforcoursedetails AS
    SELECT b.comment,
      a.commentsforcourseid,
      a.courseid,
      b.standardcommentid

        FROM (public.commentsforcourse a
            JOIN public.standardcomments b ON ((a.standardcommentid = b.standardcommentid)))
      ORDER BY b.comment;

CREATE TABLE public.ingredientcategory (
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL
);


CREATE SEQUENCE public.subcategory_mapping_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;

CREATE TABLE public.subcategorymapping (
    subcategorymappingid integer NOT NULL,
    fatherid integer NOT NULL,
    sonid integer NOT NULL
);


ALTER TABLE ONLY public.subcategorymapping ALTER COLUMN subcategorymappingid SET DEFAULT nextval('public.subcategory_mapping_seq');

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT coursecommentid_pair UNIQUE (fatherid,sonid);

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT subcategorymapping_pkey PRIMARY KEY (subcategorymappingid);


ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT father_fk FOREIGN KEY (fatherid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT son_fk FOREIGN KEY (sonid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;

--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredient_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


CREATE VIEW public.fathersoncategoriesdetails AS
    SELECT a.categoryid,
        a.name as fathername,
        a.categoryid as fatherid,
        c.name as sonname,
        c.categoryid as sonid,
        b.subcategorymappingid,
        a.visibile as fathervisible,
        c.visibile as sonvisible
    FROM (((public.coursecategories a
        JOIN public.subcategorymapping b ON ((a.categoryid=b.fatherid)))                
        JOIN public.coursecategories c ON ((c.categoryid=b.sonid))));


     


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredient_categoryid_seq OWNED BY public.ingredientcategory.ingredientcategoryid;


--
-- Name: ingredientcourse; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientcourse (
    ingredientcourseid integer NOT NULL,
    courseid integer NOT NULL,
    ingredientid integer NOT NULL,
    quantity numeric(10,2)
);





--
-- Name: ingredientcourseid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientcourseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: ingredientcourseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredientcourseid_seq OWNED BY public.ingredientcourse.ingredientcourseid;


--
-- Name: ingredientdecrement; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientdecrement (
    ingredientdecrementid integer DEFAULT nextval('public.incredientdecrementid_seq'::regclass) NOT NULL,
    orderitemid integer NOT NULL,
    typeofdecrement character varying(30) NOT NULL,
    presumednormalquantity numeric(10,2),
    recordedquantity numeric(10,2),
    preparatorid integer NOT NULL,
    registrationtime timestamp without time zone NOT NULL,
    ingredientid integer NOT NULL
);



--
-- Name: orderitems; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderitems (
    orderitemid integer NOT NULL,
    courseid integer NOT NULL,
    quantity integer NOT NULL,
    orderid integer NOT NULL,
    comment character varying(50),
    price numeric(10,2),
    stateid integer NOT NULL,
    archived boolean,
    startingtime timestamp without time zone NOT NULL,
    closingtime timestamp without time zone,
    ordergroupid integer NOT NULL,
    hasbeenrejected boolean NOT NULL,
    suborderid integer,
    isinsasuborder boolean DEFAULT false,
    printcount integer NOT NULL
);



--
-- Name: ingredientdecrementview; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientdecrementview AS
 SELECT a.ingredientdecrementid,
    a.orderitemid,
    c.quantity AS numberofcourses,
    c.closingtime,
    c.courseid,
    d.name AS coursename,
    a.typeofdecrement,
    a.presumednormalquantity,
    a.recordedquantity,
    a.registrationtime,
    a.preparatorid,
    a.ingredientid,
    b.updateavailabilityflag,
    b.checkavailabilityflag,
    b.availablequantity,
    b.name AS ingredientname,
    e.quantity AS ingredientquantity,
    b.unitmeasure
   FROM ((((public.ingredientdecrement a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.orderitems c ON ((a.orderitemid = c.orderitemid)))
     JOIN public.courses d ON ((c.courseid = d.courseid)))
     LEFT JOIN public.ingredientcourse e ON (((e.ingredientid = a.ingredientid) AND (e.courseid = c.courseid))));



--
-- Name: ingredientdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientdetails AS
 SELECT b.ingredientid,
    b.name AS ingredientname,
    b.allergen,
    b.visibility,
    d.visibility AS categoryvisibility,
    d.ingredientcategoryid
   FROM (public.ingredient b
     JOIN public.ingredientcategory d ON ((b.ingredientcategoryid = d.ingredientcategoryid)));



--
-- Name: ingredientid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: ingredientid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.ingredientid_seq OWNED BY public.ingredient.ingredientid;


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientincrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: ingredientincrement; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientincrement (
    ingredientincrementid integer DEFAULT nextval('public.ingredientincrementid_seq'::regclass) NOT NULL,
    ingredientid integer NOT NULL,
    comment character varying(100) NOT NULL,
    unitofmeasure character varying(10) NOT NULL,
    quantity numeric(10,2) NOT NULL,
    userid integer NOT NULL,
    registrationtime timestamp without time zone NOT NULL
);



--
-- Name: ingredientofcourses; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientofcourses AS
 SELECT a.courseid,
    b.ingredientid,
    a.name AS coursename,
    b.name AS ingredientname,
    c.quantity,
    b.availablequantity,
    c.ingredientcourseid,
    b.allergen,
    b.visibility,
    d.visibility AS categoryvisibility,
    b.checkavailabilityflag,
    d.ingredientcategoryid,
    b.unitmeasure
   FROM (((public.courses a
     JOIN public.ingredientcourse c ON ((a.courseid = c.courseid)))
     JOIN public.ingredient b ON ((c.ingredientid = b.ingredientid)))
     JOIN public.ingredientcategory d ON ((b.ingredientcategoryid = d.ingredientcategoryid)));



--
-- Name: ingredientpriceid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.ingredientpriceid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: ingredientprice; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.ingredientprice (
    ingredientpriceid integer DEFAULT nextval('public.ingredientpriceid_seq'::regclass) NOT NULL,
    ingredientid integer NOT NULL,
    quantity numeric(10,2) NOT NULL,
    isdefaultadd boolean NOT NULL,
    isdefaultsubtract boolean NOT NULL,
    addprice numeric(10,2) NOT NULL,
    subtractprice numeric(10,2) NOT NULL
);



--
-- Name: ingredientpricedetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.ingredientpricedetails AS
 SELECT a.ingredientpriceid,
    a.ingredientid,
    a.quantity,
    a.isdefaultadd,
    a.isdefaultsubtract,
    a.addprice,
    a.subtractprice,
    b.name
   FROM (public.ingredientprice a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
  ORDER BY b.name;



--
-- Name: invoicesid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.invoicesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: invoices; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.invoices (
    invoicesid integer DEFAULT nextval('public.invoicesid_seq'::regclass) NOT NULL,
    data character varying(4000) NOT NULL,
    invoicenumber integer NOT NULL,
    customerdataid integer,
    date timestamp without time zone NOT NULL,
    suborderid integer,
    orderid integer
);



--
-- Name: orders; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orders (
    orderid integer NOT NULL,
    "table" character varying(100) NOT NULL,
    person character varying(120) NOT NULL,
    ongoing boolean DEFAULT true,
    userid integer NOT NULL,
    startingtime timestamp without time zone NOT NULL,
    closingtime timestamp without time zone,
    voided boolean DEFAULT false,
    archived boolean DEFAULT false NOT NULL,
    total numeric(10,2),
    adjustedtotal numeric(10,2),
    plaintotalvariation numeric(10,2) DEFAULT 0,
    percentagevariataion numeric(10,2) DEFAULT 0,
    adjustispercentage boolean DEFAULT false,
    adjustisplain boolean DEFAULT false,
    forqruserarchived boolean
);



--
-- Name: users; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.users (
    userid integer NOT NULL,
    username character varying(200) NOT NULL,
    password character varying(200) NOT NULL,
    enabled boolean DEFAULT true NOT NULL,
    canvoidorders boolean DEFAULT false,
    role integer NOT NULL,
    canmanageallorders boolean DEFAULT false NOT NULL,
    creationtime timestamp without time zone,
    istemporary boolean NOT NULL,
    canchangetheprice boolean NOT NULL,
    "table" character varying(200),
    consumed boolean,
    canmanagecourses boolean NOT NULL
);



--
-- Name: nonarchivedorderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.nonarchivedorderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  WHERE ((a.archived = false) AND (a.voided = false) AND (( SELECT count(*) AS count
           FROM (public.orderitems c
             JOIN public.states d ON ((c.stateid = d.stateid)))
          WHERE ((c.orderid = a.orderid) AND (NOT d.isinitial))) > 0))
  ORDER BY a.startingtime;



--
-- Name: nonemptyorderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.nonemptyorderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  WHERE (( SELECT count(*) AS count
           FROM public.orderitems c
          WHERE (c.orderid = a.orderid)) > 0)
  ORDER BY a.startingtime;



--
-- Name: observers_observerid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observerid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: observers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.observers (
    observersid integer DEFAULT nextval('public.observers_observerid_seq'::regclass) NOT NULL,
    stateid integer NOT NULL,
    roleid integer NOT NULL,
    categoryid integer NOT NULL
);



--
-- Name: observers_observersid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.observers_observersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: observersrolestatuscategories; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.observersrolestatuscategories AS
 SELECT a.observersid,
    c.rolename,
    d.name AS categoryname,
    e.statusname
   FROM (((public.observers a
     JOIN public.roles c ON ((a.roleid = c.roleid)))
     JOIN public.coursecategories d ON ((a.categoryid = d.categoryid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));



--
-- Name: orderdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderdetails AS
 SELECT a.orderid,
    a."table",
    a.person,
    a.ongoing,
    a.userid,
    a.startingtime,
    a.closingtime,
    a.voided,
    a.total,
    a.adjustedtotal,
    a.archived,
    b.username,
    a.forqruserarchived
   FROM (public.orders a
     JOIN public.users b ON ((a.userid = b.userid)))
  ORDER BY a.startingtime;



--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderoutgroup_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: orderoutgroup; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderoutgroup (
    ordergroupid integer DEFAULT nextval('public.orderoutgroup_id_seq'::regclass) NOT NULL,
    printcount integer NOT NULL,
    orderid integer NOT NULL,
    groupidentifier integer NOT NULL
);



--
-- Name: suborderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.suborderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: suborder; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.suborder (
    suborderid integer DEFAULT nextval('public.suborderid_seq'::regclass) NOT NULL,
    orderid integer NOT NULL,
    subtotal numeric(10,2) NOT NULL,
    comment character varying(30),
    payed boolean NOT NULL,
    creationtime timestamp without time zone NOT NULL,
    tendercodesid integer,
    subtotaladjustment numeric(10,2) DEFAULT 0,
    subtotalpercentadjustment numeric(10,2) DEFAULT 0
);



--
-- Name: orderitemdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderitemdetails AS
 SELECT a.orderitemid,
    a.comment,
    a.orderid,
    a.quantity,
    b.name,
    b.price AS originalprice,
    a.price,
    b.categoryid,
    b.courseid,
    c.statusname,
    a.stateid,
    d.userid,
    a.startingtime,
    a.closingtime,
    d."table",
    d.person,
    e.username,
    a.hasbeenrejected,
    a.suborderid,
    a.isinsasuborder,
    g.groupidentifier,
    g.ordergroupid,
    (COALESCE(f.payed, false) OR d.archived) AS payed
   FROM ((((((public.orderitems a
     JOIN public.courses b ON ((a.courseid = b.courseid)))
     JOIN public.states c ON ((a.stateid = c.stateid)))
     JOIN public.orders d ON ((d.orderid = a.orderid)))
     JOIN public.users e ON ((d.userid = e.userid)))
     JOIN public.orderoutgroup g ON ((a.ordergroupid = g.ordergroupid)))
     LEFT JOIN public.suborder f ON ((a.suborderid = f.suborderid)));



--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitems_orderitemid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orderitems_orderitemid_seq OWNED BY public.orderitems.orderitemid;


--
-- Name: orderitemstates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.orderitemstates (
    orderitemstatesid integer NOT NULL,
    orderitemid integer NOT NULL,
    stateid integer NOT NULL,
    startingtime timestamp without time zone NOT NULL
);



--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orderitemstates_orderitemstates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orderitemstates_orderitemstates_id_seq OWNED BY public.orderitemstates.orderitemstatesid;


--
-- Name: orderoutgroupdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.orderoutgroupdetails AS
 SELECT a.ordergroupid,
    a.printcount,
    a.orderid,
    a.groupidentifier,
    b."table",
    b.person
   FROM (public.orderoutgroup a
     JOIN public.orders b ON ((a.orderid = b.orderid)));



--
-- Name: orders_orderid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.orders_orderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: orders_orderid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.orders_orderid_seq OWNED BY public.orders.orderid;


--
-- Name: paymentid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.paymentid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: paymentitem; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.paymentitem (
    paymentid integer DEFAULT nextval('public.paymentid_seq'::regclass) NOT NULL,
    suborderid integer,
    orderid integer,
    tendercodesid integer NOT NULL,
    amount numeric(10,2) NOT NULL
);



--
-- Name: tendercodesid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.tendercodesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: tendercodes; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.tendercodes (
    tendercodesid integer DEFAULT nextval('public.tendercodesid_seq'::regclass) NOT NULL,
    tendercode integer NOT NULL,
    tendername character varying(50) NOT NULL
);



--
-- Name: paymentitemdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.paymentitemdetails AS
 SELECT a.paymentid,
    a.suborderid,
    a.orderid,
    a.tendercodesid,
    a.amount,
    b.tendername,
    b.tendercode AS tendercodeidentifier
   FROM (public.paymentitem a
     JOIN public.tendercodes b ON ((a.tendercodesid = b.tendercodesid)));



--
-- Name: printerforcategory_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printerforcategory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: printerforcategory; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printerforcategory (
    printerforcategoryid integer DEFAULT nextval('public.printerforcategory_id_seq'::regclass) NOT NULL,
    categoryid integer NOT NULL,
    printerid integer NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: printers_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printers_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: printers; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printers (
    printerid integer DEFAULT nextval('public.printers_id_seq'::regclass) NOT NULL,
    name character varying(60) NOT NULL
);



--
-- Name: printerforcategorydetail; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.printerforcategorydetail AS
 SELECT a.printerforcategoryid,
    a.categoryid,
    a.printerid,
    a.stateid,
    b.name AS categoryname,
    c.name AS printername,
    e.statusname AS statename
   FROM (((public.printerforcategory a
     JOIN public.coursecategories b ON ((a.categoryid = b.categoryid)))
     JOIN public.printers c ON ((a.printerid = c.printerid)))
     JOIN public.states e ON ((a.stateid = e.stateid)));



--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.printerforreceiptandinvoice_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: printerforreceiptandinvoice; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.printerforreceiptandinvoice (
    printerforcategoryid integer DEFAULT nextval('public.printerforreceiptandinvoice_id_seq'::regclass) NOT NULL,
    printinvoice boolean DEFAULT false NOT NULL,
    printreceipt boolean DEFAULT false NOT NULL,
    printerid integer NOT NULL
);



--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.rejectedorderitems_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: rejectedorderitems; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.rejectedorderitems (
    rejectedorderitemid integer DEFAULT nextval('public.rejectedorderitems_id_seq'::regclass) NOT NULL,
    courseid integer NOT NULL,
    cause character varying(100) NOT NULL,
    timeofrejection timestamp without time zone NOT NULL,
    orderitemid integer NOT NULL
);



--
-- Name: roles_roleid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.roles_roleid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: roles_roleid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.roles_roleid_seq OWNED BY public.roles.roleid;


--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.tempuseractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: temp_user_actionable_states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.temp_user_actionable_states (
    tempuseractionablestateid integer DEFAULT nextval('public.tempuseractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.temp_user_actionable_states_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: temp_user_default_actionable_states; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.temp_user_default_actionable_states (
    tempmuseractionablestatesid integer DEFAULT nextval('public.temp_user_actionable_states_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.users_userid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- Name: usersview; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.usersview AS
 SELECT a.userid,
    a.username,
    a.password,
    a.enabled,
    a.canvoidorders,
    a.role,
    b.rolename,
    a.istemporary,
    a.creationtime,
    a.consumed,
    a.canmanagecourses,
    a.canmanageallorders
   FROM (public.users a
     JOIN public.roles b ON ((a.role = b.roleid)))
  ORDER BY a.username;



--
-- Name: variations; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.variations (
    variationsid integer NOT NULL,
    orderitemid integer NOT NULL,
    ingredientid integer NOT NULL,
    tipovariazione character varying(30) NOT NULL,
    plailnumvariation integer,
    ingredientpriceid integer
);



--
-- Name: variationdetails; Type: VIEW; Schema: public; Owner: Tonyx
--

CREATE VIEW public.variationdetails AS
 SELECT a.variationsid,
    a.orderitemid,
    a.ingredientid,
    a.tipovariazione,
    b.name AS ingredientname,
    b.allergen,
    a.plailnumvariation,
    c.courseid,
    d.userid,
    a.ingredientpriceid,
    e.quantity
   FROM ((((public.variations a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.orderitems c ON ((a.orderitemid = c.orderitemid)))
     JOIN public.orders d ON ((c.orderid = d.orderid)))
     LEFT JOIN public.ingredientprice e ON ((a.ingredientpriceid = e.ingredientpriceid)))
  ORDER BY b.name;



--
-- Name: variations_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.variations_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: variations_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.variations_seq OWNED BY public.variations.variationsid;


--
-- Name: voidedorderslogbuffer; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.voidedorderslogbuffer (
    voidedorderslogbufferid integer NOT NULL,
    voidedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL,
    userid integer NOT NULL
);



--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.voidedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: Tonyx
--

ALTER SEQUENCE public.voidedorderslog_id_seq OWNED BY public.voidedorderslogbuffer.voidedorderslogbufferid;


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: Tonyx
--

CREATE SEQUENCE public.waiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;



--
-- Name: waiteractionablestates; Type: TABLE; Schema: public; Owner: Tonyx
--

CREATE TABLE public.waiteractionablestates (
    waiterwatchablestatesid integer DEFAULT nextval('public.waiteractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);



--
-- Name: archivedorderslogbuffer archivedlogbufferid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer ALTER COLUMN archivedlogbufferid SET DEFAULT nextval('public.archivedorderslog_id_seq'::regclass);


--
-- Name: coursecategories categoryid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories ALTER COLUMN categoryid SET DEFAULT nextval('public.courses_categoryid_seq'::regclass);


--
-- Name: courses courseid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses ALTER COLUMN courseid SET DEFAULT nextval('public.courses_courseid_seq'::regclass);


--
-- Name: ingredient ingredientid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient ALTER COLUMN ingredientid SET DEFAULT nextval('public.ingredientid_seq'::regclass);


--
-- Name: ingredientcategory ingredientcategoryid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory ALTER COLUMN ingredientcategoryid SET DEFAULT nextval('public.ingredient_categoryid_seq'::regclass);


--
-- Name: ingredientcourse ingredientcourseid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse ALTER COLUMN ingredientcourseid SET DEFAULT nextval('public.ingredientcourseid_seq'::regclass);


--
-- Name: orderitems orderitemid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems ALTER COLUMN orderitemid SET DEFAULT nextval('public.orderitems_orderitemid_seq'::regclass);


--
-- Name: orderitemstates orderitemstatesid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates ALTER COLUMN orderitemstatesid SET DEFAULT nextval('public.orderitemstates_orderitemstates_id_seq'::regclass);


--
-- Name: orders orderid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders ALTER COLUMN orderid SET DEFAULT nextval('public.orders_orderid_seq'::regclass);


--
-- Name: roles roleid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles ALTER COLUMN roleid SET DEFAULT nextval('public.roles_roleid_seq'::regclass);


--
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- Name: variations variationsid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations ALTER COLUMN variationsid SET DEFAULT nextval('public.variations_seq'::regclass);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid; Type: DEFAULT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer ALTER COLUMN voidedorderslogbufferid SET DEFAULT nextval('public.voidedorderslog_id_seq'::regclass);


--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 282, true);


--
-- Data for Name: archivedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.archivedorderslogbuffer (archivedlogbufferid, archivedtime, orderid) FROM stdin;
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.coursecategories (categoryid, name, visibile, abstract) FROM stdin;
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.courses (courseid, name, description, price, categoryid, visibility) FROM stdin;
\.


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 69, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 535, true);


--
-- Data for Name: customerdata; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.customerdata (customerdataid, data, name) FROM stdin;
\.


--
-- Name: customerdata_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.customerdata_id_seq', 12, true);


--
-- Data for Name: defaultactionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.defaultactionablestates (defaultactionablestatesid, stateid) FROM stdin;
\.


--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.defaulwaiteractionablestates_seq', 30, true);


--
-- Data for Name: enablers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.enablers (enablersid, roleid, stateid, categoryid) FROM stdin;
\.


--
-- Name: enablers_elablersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 190, true);


--
-- Name: incredientdecrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.incredientdecrementid_seq', 270, true);


--
-- Data for Name: ingredient; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredient (ingredientid, ingredientcategoryid, name, description, visibility, allergen, updateavailabilityflag, availablequantity, checkavailabilityflag, unitmeasure) FROM stdin;
180	56	green salad		t	f	f	0.00	f	gr
\.


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredient_categoryid_seq', 56, true);


--
-- Data for Name: ingredientcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientcategory (ingredientcategoryid, name, description, visibility) FROM stdin;
56	vegetables		t
\.


--
-- Data for Name: ingredientcourse; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientcourse (ingredientcourseid, courseid, ingredientid, quantity) FROM stdin;
\.


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 362, true);


--
-- Data for Name: ingredientdecrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientdecrement (ingredientdecrementid, orderitemid, typeofdecrement, presumednormalquantity, recordedquantity, preparatorid, registrationtime, ingredientid) FROM stdin;
\.


--
-- Name: ingredientid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientid_seq', 180, true);


--
-- Data for Name: ingredientincrement; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientincrement (ingredientincrementid, ingredientid, comment, unitofmeasure, quantity, userid, registrationtime) FROM stdin;
\.


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientincrementid_seq', 32, true);


--
-- Data for Name: ingredientprice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.ingredientprice (ingredientpriceid, ingredientid, quantity, isdefaultadd, isdefaultsubtract, addprice, subtractprice) FROM stdin;
\.


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 98, true);


--
-- Data for Name: invoices; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.invoices (invoicesid, data, invoicenumber, customerdataid, date, suborderid, orderid) FROM stdin;
\.


--
-- Name: invoicesid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.invoicesid_seq', 45, true);


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
\.


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 210, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, suborderid, isinsasuborder, printcount) FROM stdin;
\.


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1382, true);


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
\.


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 2594, true);


--
-- Data for Name: orderoutgroup; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orderoutgroup (ordergroupid, printcount, orderid, groupidentifier) FROM stdin;
\.


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 499, true);


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
\.


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 587, true);


--
-- Name: paymentid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.paymentid_seq', 115, true);


--
-- Data for Name: paymentitem; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.paymentitem (paymentid, suborderid, orderid, tendercodesid, amount) FROM stdin;
\.


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
\.


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 55, true);


--
-- Data for Name: printerforreceiptandinvoice; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printerforreceiptandinvoice (printerforcategoryid, printinvoice, printreceipt, printerid) FROM stdin;
\.


--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printerforreceiptandinvoice_id_seq', 4, true);


--
-- Data for Name: printers; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.printers (printerid, name) FROM stdin;
\.


--
-- Name: printers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.printers_id_seq', 41, true);


--
-- Data for Name: rejectedorderitems; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.rejectedorderitems (rejectedorderitemid, courseid, cause, timeofrejection, orderitemid) FROM stdin;
\.


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 81, true);


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.roles (roleid, rolename, comment) FROM stdin;
1	admin	\N
\.


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 27, true);


--
-- Data for Name: states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.states (stateid, isinitial, isfinal, statusname, nextstateid, isexceptional, creatingingredientdecrement) FROM stdin;
1	t	f	COLLECTING	2	f	f
6	f	t	DONE	\N	f	f
2	f	f	TOBEWORKED	6	f	t
\.


--
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.states_stateid_seq', 6, true);


--
-- Data for Name: suborder; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.suborder (suborderid, orderid, subtotal, comment, payed, creationtime, tendercodesid, subtotaladjustment, subtotalpercentadjustment) FROM stdin;
\.


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.suborderid_seq', 316, true);


--
-- Data for Name: temp_user_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_actionable_states (tempuseractionablestateid, userid, stateid) FROM stdin;
\.


--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.temp_user_actionable_states_seq', 12, true);


--
-- Data for Name: temp_user_default_actionable_states; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.temp_user_default_actionable_states (tempmuseractionablestatesid, stateid) FROM stdin;
\.


--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.tempuseractionablestates_seq', 1, false);


--
-- Data for Name: tendercodes; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.tendercodes (tendercodesid, tendercode, tendername) FROM stdin;
1	1	CONTANTI
2	2	CREDITO
3	3	ASSEGNI
5	4	BUONI
6	5	CARTA DI CREDITO
\.


--
-- Name: tendercodesid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.tendercodesid_seq', 6, true);


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.users (userid, username, password, enabled, canvoidorders, role, canmanageallorders, creationtime, istemporary, canchangetheprice, "table", consumed, canmanagecourses) FROM stdin;
2	administrator	4194d1706ed1f408d5e02d672777019f4d5385c766a8c6ca8acba3167d36a7b9	t	t	1	t	\N	f	t	\N	\N	f
\.


--
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.users_userid_seq', 174, true);


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
\.


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.variations_seq', 1079, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 307, true);


--
-- Data for Name: voidedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.voidedorderslogbuffer (voidedorderslogbufferid, voidedtime, orderid, userid) FROM stdin;
\.


--
-- Data for Name: waiteractionablestates; Type: TABLE DATA; Schema: public; Owner: Tonyx
--

COPY public.waiteractionablestates (waiterwatchablestatesid, userid, stateid) FROM stdin;
\.


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: Tonyx
--

SELECT pg_catalog.setval('public.waiteractionablestates_seq', 300, true);


--
-- Name: archivedorderslogbuffer archivedlogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT archivedlogbufferid_key PRIMARY KEY (archivedlogbufferid);


--
-- Name: coursecategories category_uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT category_uniquename UNIQUE (name);


--
-- Name: coursecategories coursecategories_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT coursecategories_pkey PRIMARY KEY (categoryid);


--
-- Name: courses courses_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT courses_pkey PRIMARY KEY (courseid);


--
-- Name: customerdata customer_name_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customer_name_unique UNIQUE (name);


--
-- Name: customerdata customerdata_name_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_name_key UNIQUE (name);


--
-- Name: customerdata customerdata_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_pkey PRIMARY KEY (customerdataid);


--
-- Name: defaultactionablestates defaultactionablestateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestateidunique UNIQUE (stateid);


--
-- Name: defaultactionablestates defaultactionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_pkey PRIMARY KEY (defaultactionablestatesid);


--
-- Name: enablers enablers_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_pkey PRIMARY KEY (enablersid);


--
-- Name: enablers enablers_tripleidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: ingredientcategory ing_cat_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ing_cat_unique_name UNIQUE (name);


--
-- Name: ingredient ing_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ing_unique_name UNIQUE (name);


--
-- Name: ingredientdecrement ingredient_decrement_unique_ordit_ingid; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_decrement_unique_ordit_ingid UNIQUE (orderitemid, ingredientid);


--
-- Name: ingredient ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredient_pkey PRIMARY KEY (ingredientid);


--
-- Name: ingredientcategory ingredientcateogory_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ingredientcateogory_pkey PRIMARY KEY (ingredientcategoryid);


--
-- Name: ingredientcourse ingredientcourse_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_pkey PRIMARY KEY (ingredientcourseid);


--
-- Name: ingredientdecrement ingredientincrement_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredientincrement_key PRIMARY KEY (ingredientdecrementid);


--
-- Name: ingredientincrement ingredientincrementid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT ingredientincrementid_key PRIMARY KEY (ingredientincrementid);


--
-- Name: ingredientprice ingredientprice_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_pkey PRIMARY KEY (ingredientpriceid);


--
-- Name: ingredientprice ingredientprice_uniq; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_uniq UNIQUE (ingredientid, quantity);


--
-- Name: invoices invoices_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoices_pkey PRIMARY KEY (invoicesid);


--
-- Name: observers observers_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT observers_pkey PRIMARY KEY (observersid);


--
-- Name: variations orderitem_ingredient_uniq; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT orderitem_ingredient_uniq UNIQUE (orderitemid, ingredientid);


--
-- Name: orderitems orderitems_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitems_pkey PRIMARY KEY (orderitemid);




--
-- Name: orderitemstates orderitemstates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitemstates_pkey PRIMARY KEY (orderitemstatesid);





--
-- Name: orderoutgroup orderoutgroup_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_key PRIMARY KEY (ordergroupid);


--
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderid);


--
-- Name: paymentitem payment_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT payment_key PRIMARY KEY (paymentid);


--
-- Name: printerforcategory print_cat_state_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_cat_state_unique UNIQUE (categoryid, printerid, stateid);


--
-- Name: printers printer_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printer_unique_name UNIQUE (name);


--
-- Name: printerforcategory printerforcategory_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printerforcategory_key PRIMARY KEY (printerforcategoryid);


--
-- Name: printerforreceiptandinvoice printerforreceiptandinvoice_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT printerforreceiptandinvoice_pkey PRIMARY KEY (printerforcategoryid);


--
-- Name: printers printers_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printers_key PRIMARY KEY (printerid);


--
-- Name: rejectedorderitems rejecteddorderitemhistoryid_pk; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejecteddorderitemhistoryid_pk PRIMARY KEY (rejectedorderitemid);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (roleid);


--
-- Name: roles roles_rolename_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_rolename_key UNIQUE (rolename);


--
-- Name: states state_uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT state_uniquename UNIQUE (statusname);


--
-- Name: states stateid_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT stateid_pkey PRIMARY KEY (stateid);


--
-- Name: states states_statusname_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT states_statusname_key UNIQUE (statusname);


--
-- Name: suborder suborderid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderid_key PRIMARY KEY (suborderid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_pkey PRIMARY KEY (tempmuseractionablestatesid);


--
-- Name: temp_user_actionable_states temp_user_actionable_states_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_pkey PRIMARY KEY (tempuseractionablestateid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_unique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_unique UNIQUE (stateid);


--
-- Name: temp_user_actionable_states tempuserstateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuserstateidunique UNIQUE (stateid, userid);


--
-- Name: tendercodes tendercode_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercode_key PRIMARY KEY (tendercodesid);


--
-- Name: tendercodes tendercodes_unique_code; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_code UNIQUE (tendercode);


--
-- Name: tendercodes tendercodes_unique_name; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_name UNIQUE (tendername);


--
-- Name: observers tripleidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: orderoutgroup unique_group_order; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT unique_group_order UNIQUE (orderid, groupidentifier);


--
-- Name: ingredientcourse unique_ing_courrse; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT unique_ing_courrse UNIQUE (courseid, ingredientid);


--
-- Name: courses uniquename; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT uniquename UNIQUE (name);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- Name: variations variations_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variations_pkey PRIMARY KEY (variationsid);


--
-- Name: voidedorderslogbuffer voided_ord_uniq_user_order; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_uniq_user_order UNIQUE (userid, orderid);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voidedorderslogbufferid_key PRIMARY KEY (voidedorderslogbufferid);


--
-- Name: waiteractionablestates waiteractionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiteractionablestates_pkey PRIMARY KEY (waiterwatchablestatesid);


--
-- Name: waiteractionablestates waiterdstateidunique; Type: CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterdstateidunique UNIQUE (stateid, userid);


--
-- Name: observers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: enablers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: variations category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT category_fk FOREIGN KEY (ingredientpriceid) REFERENCES public.ingredientprice(ingredientpriceid) MATCH FULL;


--
-- Name: printerforcategory category_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.printerforcategory
--    ADD CONSTRAINT category_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL;



ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT category_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL ON DELETE CASCADE;



--
-- Name: courses categoryfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT categoryfk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: orderitems coursefk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderitems
 --   ADD CONSTRAINT coursefk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT coursefk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;



--
-- Name: defaultactionablestates defaultactionablestates_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: enablers enablersrole; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablersrole FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: ingredientincrement ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredientdecrement ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredient ingredientcategory_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredientcategory_fk FOREIGN KEY (ingredientcategoryid) REFERENCES public.ingredientcategory(ingredientcategoryid) MATCH FULL;


--
-- Name: ingredientcourse ingredientcourse_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.ingredientcourse
--    ADD CONSTRAINT ingredientcourse_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;


ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientcourse ingredientcourse_ingredient_fk2; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_ingredient_fk2 FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: ingredientprice ingredientprice_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: invoices invoice_customer_data_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoice_customer_data_fk FOREIGN KEY (customerdataid) REFERENCES public.customerdata(customerdataid) MATCH FULL;


--
-- Name: printerforreceiptandinvoice invoice_receipt_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--


--ALTER TABLE ONLY public.printerforreceiptandinvoice
--    ADD CONSTRAINT invoice_receipt_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL;


ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT invoice_receipt_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL ON DELETE CASCADE;


--
-- Name: paymentitem oorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.paymentitem
--    ADD CONSTRAINT oorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT oorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: archivedorderslogbuffer order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.archivedorderslogbuffer
--    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: invoices order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.invoices
--    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitems orderdetail_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderitems
--    ADD CONSTRAINT orderdetail_fk FOREIGN KEY (ordergroupid) REFERENCES public.orderoutgroup(ordergroupid) MATCH FULL;


ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderdetail_fk FOREIGN KEY (ordergroupid) REFERENCES public.orderoutgroup(ordergroupid) MATCH FULL ON DELETE CASCADE;



--
-- Name: orderitems orderfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderitems
 --   ADD CONSTRAINT orderfk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderfk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT suborderfk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE set null;




--
-- Name: orderitemstates orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderitemstates
--    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientdecrement orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.ingredientdecrement
--    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;



--
-- Name: orderitems orderitemstatus; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitemstatus FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: orderoutgroup orderoutgroup_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderoutgroup
--    ADD CONSTRAINT orderoutgroup_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientdecrement preparator_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.ingredientdecrement
--    ADD CONSTRAINT preparator_fk FOREIGN KEY (preparatorid) REFERENCES public.users(userid) MATCH FULL;

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT preparator_fk FOREIGN KEY (preparatorid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;



--
-- Name: printerforcategory print_category_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL;


--
-- Name: printerforcategory printer_cat_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printer_cat_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: rejectedorderitems reject_orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.rejectedorderitems
--    ADD CONSTRAINT reject_orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT reject_orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;



--
-- Name: rejectedorderitems rejected_item_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.rejectedorderitems
--    ADD CONSTRAINT rejected_item_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL;


ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejected_item_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: observers role1_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT role1_fk FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: users rolefk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT rolefk FOREIGN KEY (role) REFERENCES public.roles(roleid) MATCH FULL;


--
-- Name: observers state1_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT state1_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: orderitemstates state2_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.orderitemstates
--    ADD CONSTRAINT state2_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT state2_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL 
    ON DELETE CASCADE;



ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE CASCADE;

--
-- Name: enablers state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: invoices suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL;



--
-- Name: paymentitem suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.paymentitem
--    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL;

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: suborder suborderorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.suborder
--    ADD CONSTRAINT suborderorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: paymentitem tendercode_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.paymentitem
   ADD CONSTRAINT tendercode_fk FOREIGN KEY (tendercodesid) REFERENCES public.tendercodes(tendercodesid) MATCH FULL;



--
-- Name: ingredientincrement user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.ingredientincrement
--    ADD CONSTRAINT user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orders userfk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT userfk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: variations variationingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL;


--
-- Name: variations variationorderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.variations
 --   ADD CONSTRAINT variationorderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL;


ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationorderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: voidedorderslogbuffer voided_ord_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: voidedorderslogbuffer voided_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

--ALTER TABLE ONLY public.voidedorderslogbuffer
--    ADD CONSTRAINT voided_order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL;


ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: waiteractionablestates waiterstate_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstate_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: waiteractionablestates waiterstateuser_fk; Type: FK CONSTRAINT; Schema: public; Owner: Tonyx
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstateuser_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL;


--
-- Name: TABLE archivedorderslogbuffer; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.archivedorderslogbuffer TO suave;


--
-- Name: SEQUENCE archivedorderslog_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.archivedorderslog_id_seq TO suave;


--
-- Name: TABLE coursecategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.coursecategories TO suave;


--
-- Name: TABLE courses; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.courses TO suave;


--
-- Name: TABLE coursedetails2; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.coursedetails2 TO suave;


--
-- Name: SEQUENCE courses_categoryid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.courses_categoryid_seq TO suave;


--
-- Name: SEQUENCE courses_courseid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.courses_courseid_seq TO suave;


--
-- Name: SEQUENCE customerdata_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.customerdata_id_seq TO suave;


--
-- Name: TABLE customerdata; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.customerdata TO suave;


--
-- Name: SEQUENCE defaulwaiteractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.defaulwaiteractionablestates_seq TO suave;


--
-- Name: TABLE defaultactionablestates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.defaultactionablestates TO suave;


--
-- Name: SEQUENCE enablers_elablersid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.enablers_elablersid_seq TO suave;


--
-- Name: TABLE enablers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.enablers TO suave;


--
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.roles TO suave;


--
-- Name: SEQUENCE states_stateid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.states_stateid_seq TO suave;


--
-- Name: TABLE states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.states TO suave;


--
-- Name: TABLE enablersrolestatuscategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.enablersrolestatuscategories TO suave;


--
-- Name: SEQUENCE incredientdecrementid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.incredientdecrementid_seq TO suave;


--
-- Name: TABLE ingredient; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredient TO suave;


--
-- Name: TABLE ingredientcategory; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientcategory TO suave;


--
-- Name: SEQUENCE ingredient_categoryid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredient_categoryid_seq TO suave;


--
-- Name: TABLE ingredientcourse; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientcourse TO suave;


--
-- Name: SEQUENCE ingredientcourseid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientcourseid_seq TO suave;


--
-- Name: TABLE ingredientdecrement; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdecrement TO suave;


--
-- Name: TABLE orderitems; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitems TO suave;


--
-- Name: TABLE ingredientdecrementview; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdecrementview TO suave;


--
-- Name: TABLE ingredientdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientdetails TO suave;


--
-- Name: SEQUENCE ingredientid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientid_seq TO suave;


--
-- Name: SEQUENCE ingredientincrementid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientincrementid_seq TO suave;


--
-- Name: TABLE ingredientincrement; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientincrement TO suave;


--
-- Name: TABLE ingredientofcourses; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientofcourses TO suave;


--
-- Name: SEQUENCE ingredientpriceid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.ingredientpriceid_seq TO suave;


--
-- Name: TABLE ingredientprice; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientprice TO suave;


--
-- Name: TABLE ingredientpricedetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.ingredientpricedetails TO suave;


--
-- Name: SEQUENCE invoicesid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.invoicesid_seq TO suave;


--
-- Name: TABLE invoices; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.invoices TO suave;


--
-- Name: TABLE orders; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orders TO suave;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.users TO suave;


--
-- Name: TABLE nonarchivedorderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.nonarchivedorderdetails TO suave;


--
-- Name: TABLE nonemptyorderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.nonemptyorderdetails TO suave;


--
-- Name: SEQUENCE observers_observerid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.observers_observerid_seq TO suave;


--
-- Name: TABLE observers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.observers TO suave;


--
-- Name: SEQUENCE observers_observersid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.observers_observersid_seq TO suave;


--
-- Name: TABLE observersrolestatuscategories; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.observersrolestatuscategories TO suave;


--
-- Name: TABLE orderdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderdetails TO suave;


--
-- Name: SEQUENCE orderoutgroup_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderoutgroup_id_seq TO suave;


--
-- Name: TABLE orderoutgroup; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderoutgroup TO suave;


--
-- Name: SEQUENCE suborderid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.suborderid_seq TO suave;


--
-- Name: TABLE suborder; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.suborder TO suave;


--
-- Name: TABLE orderitemdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitemdetails TO suave;


--
-- Name: SEQUENCE orderitems_orderitemid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderitems_orderitemid_seq TO suave;


--
-- Name: TABLE orderitemstates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderitemstates TO suave;


--
-- Name: SEQUENCE orderitemstates_orderitemstates_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orderitemstates_orderitemstates_id_seq TO suave;


--
-- Name: TABLE orderoutgroupdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.orderoutgroupdetails TO suave;


--
-- Name: SEQUENCE orders_orderid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.orders_orderid_seq TO suave;


--
-- Name: SEQUENCE paymentid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.paymentid_seq TO suave;


--
-- Name: TABLE paymentitem; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.paymentitem TO suave;


--
-- Name: SEQUENCE tendercodesid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.tendercodesid_seq TO suave;


--
-- Name: TABLE tendercodes; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.tendercodes TO suave;


--
-- Name: TABLE paymentitemdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.paymentitemdetails TO suave;


--
-- Name: SEQUENCE printerforcategory_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printerforcategory_id_seq TO suave;


--
-- Name: TABLE printerforcategory; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforcategory TO suave;


--
-- Name: SEQUENCE printers_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printers_id_seq TO suave;


--
-- Name: TABLE printers; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printers TO suave;


--
-- Name: TABLE printerforcategorydetail; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforcategorydetail TO suave;


--
-- Name: SEQUENCE printerforreceiptandinvoice_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.printerforreceiptandinvoice_id_seq TO suave;


--
-- Name: TABLE printerforreceiptandinvoice; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.printerforreceiptandinvoice TO suave;


--
-- Name: SEQUENCE rejectedorderitems_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.rejectedorderitems_id_seq TO suave;


--
-- Name: TABLE rejectedorderitems; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.rejectedorderitems TO suave;


--
-- Name: SEQUENCE roles_roleid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.roles_roleid_seq TO suave;


--
-- Name: SEQUENCE tempuseractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.tempuseractionablestates_seq TO suave;


--
-- Name: TABLE temp_user_actionable_states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.temp_user_actionable_states TO suave;


--
-- Name: SEQUENCE temp_user_actionable_states_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.temp_user_actionable_states_seq TO suave;


--
-- Name: TABLE temp_user_default_actionable_states; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.temp_user_default_actionable_states TO suave;


--
-- Name: SEQUENCE users_userid_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.users_userid_seq TO suave;


--
-- Name: TABLE usersview; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.usersview TO suave;


--
-- Name: TABLE variations; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.variations TO suave;


--
-- Name: TABLE variationdetails; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.variationdetails TO suave;


--
-- Name: SEQUENCE variations_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.variations_seq TO suave;


--
-- Name: TABLE voidedorderslogbuffer; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.voidedorderslogbuffer TO suave;


--
-- Name: SEQUENCE voidedorderslog_id_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.voidedorderslog_id_seq TO suave;


--
-- Name: SEQUENCE waiteractionablestates_seq; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON SEQUENCE public.waiteractionablestates_seq TO suave;


--
-- Name: TABLE waiteractionablestates; Type: ACL; Schema: public; Owner: Tonyx
--

GRANT ALL ON TABLE public.waiteractionablestates TO suave;


--
-- PostgreSQL database dump complete
--


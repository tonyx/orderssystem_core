--
-- PostgreSQL database dump
--

-- Dumped from database version 14.4

-- Dumped by pg_dump version 15.4

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: public; Type: SCHEMA; Schema: -; Owner: postgres
--

-- *not* creating schema, since initdb creates it

CREATE USER SUAVE with password '1234';


ALTER SCHEMA public OWNER TO postgres;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: archivedorderslogbuffer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.archivedorderslogbuffer (
    archivedlogbufferid integer NOT NULL,
    archivedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL
);


ALTER TABLE public.archivedorderslogbuffer OWNER TO postgres;

--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.archivedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.archivedorderslog_id_seq OWNER TO postgres;

--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.archivedorderslog_id_seq OWNED BY public.archivedorderslogbuffer.archivedlogbufferid;


--
-- Name: comments_for_course_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.comments_for_course_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.comments_for_course_seq OWNER TO postgres;

--
-- Name: commentsforcourse; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.commentsforcourse (
    commentsforcourseid integer DEFAULT nextval('public.comments_for_course_seq'::regclass) NOT NULL,
    courseid integer NOT NULL,
    standardcommentid integer NOT NULL
);


ALTER TABLE public.commentsforcourse OWNER TO postgres;

--
-- Name: standard_comments_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.standard_comments_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.standard_comments_seq OWNER TO postgres;

--
-- Name: standardcomments; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.standardcomments (
    standardcommentid integer DEFAULT nextval('public.standard_comments_seq'::regclass) NOT NULL,
    comment character varying(59) NOT NULL
);


ALTER TABLE public.standardcomments OWNER TO postgres;

--
-- Name: commentsforcoursedetails; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.commentsforcoursedetails AS
 SELECT b.comment,
    a.commentsforcourseid,
    a.courseid,
    b.standardcommentid
   FROM (public.commentsforcourse a
     JOIN public.standardcomments b ON ((a.standardcommentid = b.standardcommentid)))
  ORDER BY b.comment;


ALTER TABLE public.commentsforcoursedetails OWNER TO postgres;

--
-- Name: coursecategories; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.coursecategories (
    categoryid integer NOT NULL,
    name character varying(50) NOT NULL,
    visibile boolean DEFAULT true,
    abstract boolean NOT NULL
);


ALTER TABLE public.coursecategories OWNER TO postgres;

--
-- Name: courses; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.courses (
    courseid smallint NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    price numeric(10,2) DEFAULT 0,
    categoryid integer NOT NULL,
    visibility boolean NOT NULL
);


ALTER TABLE public.courses OWNER TO postgres;

--
-- Name: coursedetails2; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.coursedetails2 AS
 SELECT a.courseid,
    a.name,
    a.description,
    a.price,
    c.name AS coursecategoryname,
    c.categoryid,
    a.visibility,
    c.visibile AS categoryvisibility
   FROM (public.courses a
     JOIN public.coursecategories c ON ((a.categoryid = c.categoryid)))
  ORDER BY a.courseid;


ALTER TABLE public.coursedetails2 OWNER TO postgres;

--
-- Name: courses_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.courses_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.courses_categoryid_seq OWNER TO postgres;

--
-- Name: courses_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.courses_categoryid_seq OWNED BY public.coursecategories.categoryid;


--
-- Name: courses_courseid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.courses_courseid_seq as smallint
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.courses_courseid_seq OWNER TO postgres;

--
-- Name: courses_courseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.courses_courseid_seq OWNED BY public.courses.courseid;


--
-- Name: customerdata_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.customerdata_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.customerdata_id_seq OWNER TO postgres;

--
-- Name: customerdata; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.customerdata (
    customerdataid integer DEFAULT nextval('public.customerdata_id_seq'::regclass) NOT NULL,
    data character varying(4000) NOT NULL,
    name character varying(300) NOT NULL
);


ALTER TABLE public.customerdata OWNER TO postgres;

--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.defaulwaiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.defaulwaiteractionablestates_seq OWNER TO postgres;

--
-- Name: defaultactionablestates; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.defaultactionablestates (
    defaultactionablestatesid integer DEFAULT nextval('public.defaulwaiteractionablestates_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.defaultactionablestates OWNER TO postgres;

--
-- Name: enablers_elablersid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.enablers_elablersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.enablers_elablersid_seq OWNER TO postgres;

--
-- Name: enablers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.enablers (
    enablersid integer DEFAULT nextval('public.enablers_elablersid_seq'::regclass) NOT NULL,
    roleid integer NOT NULL,
    stateid integer NOT NULL,
    categoryid integer NOT NULL
);


ALTER TABLE public.enablers OWNER TO postgres;

--
-- Name: roles; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roles (
    roleid integer NOT NULL,
    rolename character varying(30) NOT NULL,
    comment character varying(50)
);


ALTER TABLE public.roles OWNER TO postgres;

--
-- Name: states_stateid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.states_stateid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.states_stateid_seq OWNER TO postgres;

--
-- Name: states; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.states OWNER TO postgres;

--
-- Name: enablersrolestatuscategories; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.enablersrolestatuscategories OWNER TO postgres;

--
-- Name: subcategory_mapping_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.subcategory_mapping_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.subcategory_mapping_seq OWNER TO postgres;

--
-- Name: subcategorymapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.subcategorymapping (
    subcategorymappingid integer DEFAULT nextval('public.subcategory_mapping_seq'::regclass) NOT NULL,
    fatherid integer NOT NULL,
    sonid integer NOT NULL
);


ALTER TABLE public.subcategorymapping OWNER TO postgres;

--
-- Name: fathersoncategoriesdetails; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.fathersoncategoriesdetails AS
 SELECT a.categoryid,
    a.name AS fathername,
    a.categoryid AS fatherid,
    c.name AS sonname,
    c.categoryid AS sonid,
    b.subcategorymappingid,
    a.visibile AS fathervisible,
    c.visibile AS sonvisible
   FROM ((public.coursecategories a
     JOIN public.subcategorymapping b ON ((a.categoryid = b.fatherid)))
     JOIN public.coursecategories c ON ((c.categoryid = b.sonid)));


ALTER TABLE public.fathersoncategoriesdetails OWNER TO postgres;

--
-- Name: incredientdecrementid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.incredientdecrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.incredientdecrementid_seq OWNER TO postgres;

--
-- Name: ingredient; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredient OWNER TO postgres;

--
-- Name: ingredientcategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredientcategory (
    ingredientcategoryid integer NOT NULL,
    name character varying(120) NOT NULL,
    description character varying(4000),
    visibility boolean NOT NULL
);


ALTER TABLE public.ingredientcategory OWNER TO postgres;

--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredient_categoryid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredient_categoryid_seq OWNER TO postgres;

--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredient_categoryid_seq OWNED BY public.ingredientcategory.ingredientcategoryid;


--
-- Name: ingredientcourse; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.ingredientcourse (
    ingredientcourseid integer NOT NULL,
    courseid integer NOT NULL,
    ingredientid integer NOT NULL,
    quantity numeric(10,2)
);


ALTER TABLE public.ingredientcourse OWNER TO postgres;

--
-- Name: ingredientcourseid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredientcourseid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientcourseid_seq OWNER TO postgres;

--
-- Name: ingredientcourseid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredientcourseid_seq OWNED BY public.ingredientcourse.ingredientcourseid;


--
-- Name: ingredientdecrement; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientdecrement OWNER TO postgres;

--
-- Name: orderitems; Type: TABLE; Schema: public; Owner: postgres
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
    printcount integer NOT NULL
);


ALTER TABLE public.orderitems OWNER TO postgres;

--
-- Name: orders; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.orders OWNER TO postgres;

--
-- Name: ingredientdecrementview; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.ingredientdecrementview AS
 SELECT a.ingredientdecrementid,
    a.orderitemid,
    c.quantity AS numberofcourses,
    f.closingtime,
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
   FROM (((((public.ingredientdecrement a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.orderitems c ON ((a.orderitemid = c.orderitemid)))
     JOIN public.courses d ON ((c.courseid = d.courseid)))
     JOIN public.orders f ON ((c.orderid = f.orderid)))
     LEFT JOIN public.ingredientcourse e ON (((e.ingredientid = a.ingredientid) AND (e.courseid = c.courseid))));


ALTER TABLE public.ingredientdecrementview OWNER TO postgres;

--
-- Name: ingredientdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientdetails OWNER TO postgres;

--
-- Name: ingredientid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredientid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientid_seq OWNER TO postgres;

--
-- Name: ingredientid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.ingredientid_seq OWNED BY public.ingredient.ingredientid;


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredientincrementid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientincrementid_seq OWNER TO postgres;

--
-- Name: ingredientincrement; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientincrement OWNER TO postgres;

--
-- Name: ingredientofcourses; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientofcourses OWNER TO postgres;

--
-- Name: ingredientpriceid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.ingredientpriceid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.ingredientpriceid_seq OWNER TO postgres;

--
-- Name: ingredientprice; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientprice OWNER TO postgres;

--
-- Name: ingredientpricedetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.ingredientpricedetails OWNER TO postgres;

--
-- Name: invoicesid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.invoicesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.invoicesid_seq OWNER TO postgres;

--
-- Name: invoices; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.invoices OWNER TO postgres;

--
-- Name: users; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.users OWNER TO postgres;

--
-- Name: nonarchivedorderdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.nonarchivedorderdetails OWNER TO postgres;

--
-- Name: nonemptyorderdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.nonemptyorderdetails OWNER TO postgres;

--
-- Name: observers_observerid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.observers_observerid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.observers_observerid_seq OWNER TO postgres;

--
-- Name: observers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.observers (
    observersid integer DEFAULT nextval('public.observers_observerid_seq'::regclass) NOT NULL,
    stateid integer NOT NULL,
    roleid integer NOT NULL,
    categoryid integer NOT NULL
);


ALTER TABLE public.observers OWNER TO postgres;

--
-- Name: observers_observersid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.observers_observersid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.observers_observersid_seq OWNER TO postgres;

--
-- Name: observersrolestatuscategories; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.observersrolestatuscategories OWNER TO postgres;

--
-- Name: orderdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.orderdetails OWNER TO postgres;

--
-- Name: orderitem_sub_order_mapping_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orderitem_sub_order_mapping_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderitem_sub_order_mapping_seq OWNER TO postgres;

--
-- Name: orderitemsubordermapping; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orderitemsubordermapping (
    orderitemsubordermappingid integer DEFAULT nextval('public.orderitem_sub_order_mapping_seq'::regclass) NOT NULL,
    orderitemid integer NOT NULL,
    suborderid integer NOT NULL
);


ALTER TABLE public.orderitemsubordermapping OWNER TO postgres;

--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orderoutgroup_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderoutgroup_id_seq OWNER TO postgres;

--
-- Name: orderoutgroup; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orderoutgroup (
    ordergroupid integer DEFAULT nextval('public.orderoutgroup_id_seq'::regclass) NOT NULL,
    printcount integer NOT NULL,
    orderid integer NOT NULL,
    groupidentifier integer NOT NULL
);


ALTER TABLE public.orderoutgroup OWNER TO postgres;

--
-- Name: suborderid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.suborderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.suborderid_seq OWNER TO postgres;

--
-- Name: suborder; Type: TABLE; Schema: public; Owner: postgres
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


ALTER TABLE public.suborder OWNER TO postgres;

--
-- Name: orderitemdetails; Type: VIEW; Schema: public; Owner: postgres
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
    h.suborderid,
    g.groupidentifier,
    g.ordergroupid,
    (COALESCE(f.payed, false) OR d.archived OR d.voided) AS payed
   FROM (((((((public.orderitems a
     JOIN public.courses b ON ((a.courseid = b.courseid)))
     JOIN public.states c ON ((a.stateid = c.stateid)))
     JOIN public.orders d ON ((d.orderid = a.orderid)))
     JOIN public.users e ON ((d.userid = e.userid)))
     JOIN public.orderoutgroup g ON ((a.ordergroupid = g.ordergroupid)))
     LEFT JOIN public.orderitemsubordermapping h ON ((a.orderitemid = h.orderitemid)))
     LEFT JOIN public.suborder f ON ((f.suborderid = h.suborderid)));


ALTER TABLE public.orderitemdetails OWNER TO postgres;

--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orderitems_orderitemid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderitems_orderitemid_seq OWNER TO postgres;

--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orderitems_orderitemid_seq OWNED BY public.orderitems.orderitemid;


--
-- Name: orderitemstates; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.orderitemstates (
    orderitemstatesid integer NOT NULL,
    orderitemid integer NOT NULL,
    stateid integer NOT NULL,
    startingtime timestamp without time zone NOT NULL
);


ALTER TABLE public.orderitemstates OWNER TO postgres;

--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orderitemstates_orderitemstates_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orderitemstates_orderitemstates_id_seq OWNER TO postgres;

--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orderitemstates_orderitemstates_id_seq OWNED BY public.orderitemstates.orderitemstatesid;


--
-- Name: orderoutgroupdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.orderoutgroupdetails OWNER TO postgres;

--
-- Name: orders_orderid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.orders_orderid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.orders_orderid_seq OWNER TO postgres;

--
-- Name: orders_orderid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.orders_orderid_seq OWNED BY public.orders.orderid;


--
-- Name: paymentid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.paymentid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.paymentid_seq OWNER TO postgres;

--
-- Name: paymentitem; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.paymentitem (
    paymentid integer DEFAULT nextval('public.paymentid_seq'::regclass) NOT NULL,
    suborderid integer,
    orderid integer,
    tendercodesid integer NOT NULL,
    amount numeric(10,2) NOT NULL
);


ALTER TABLE public.paymentitem OWNER TO postgres;

--
-- Name: tendercodesid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tendercodesid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.tendercodesid_seq OWNER TO postgres;

--
-- Name: tendercodes; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.tendercodes (
    tendercodesid integer DEFAULT nextval('public.tendercodesid_seq'::regclass) NOT NULL,
    tendercode integer NOT NULL,
    tendername character varying(50) NOT NULL
);


ALTER TABLE public.tendercodes OWNER TO postgres;

--
-- Name: paymentitemdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.paymentitemdetails OWNER TO postgres;

--
-- Name: printerforcategory_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.printerforcategory_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printerforcategory_id_seq OWNER TO postgres;

--
-- Name: printerforcategory; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.printerforcategory (
    printerforcategoryid integer DEFAULT nextval('public.printerforcategory_id_seq'::regclass) NOT NULL,
    categoryid integer NOT NULL,
    printerid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.printerforcategory OWNER TO postgres;

--
-- Name: printers_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.printers_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printers_id_seq OWNER TO postgres;

--
-- Name: printers; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.printers (
    printerid integer DEFAULT nextval('public.printers_id_seq'::regclass) NOT NULL,
    name character varying(60) NOT NULL
);


ALTER TABLE public.printers OWNER TO postgres;

--
-- Name: printerforcategorydetail; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.printerforcategorydetail OWNER TO postgres;

--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.printerforreceiptandinvoice_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.printerforreceiptandinvoice_id_seq OWNER TO postgres;

--
-- Name: printerforreceiptandinvoice; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.printerforreceiptandinvoice (
    printerforcategoryid integer DEFAULT nextval('public.printerforreceiptandinvoice_id_seq'::regclass) NOT NULL,
    printinvoice boolean DEFAULT false NOT NULL,
    printreceipt boolean DEFAULT false NOT NULL,
    printerid integer NOT NULL
);


ALTER TABLE public.printerforreceiptandinvoice OWNER TO postgres;

--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.rejectedorderitems_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.rejectedorderitems_id_seq OWNER TO postgres;

--
-- Name: rejectedorderitems; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rejectedorderitems (
    rejectedorderitemid integer DEFAULT nextval('public.rejectedorderitems_id_seq'::regclass) NOT NULL,
    courseid integer NOT NULL,
    cause character varying(100) NOT NULL,
    timeofrejection timestamp without time zone NOT NULL,
    orderitemid integer NOT NULL
);


ALTER TABLE public.rejectedorderitems OWNER TO postgres;

--
-- Name: roles_roleid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roles_roleid_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.roles_roleid_seq OWNER TO postgres;

--
-- Name: roles_roleid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roles_roleid_seq OWNED BY public.roles.roleid;


--
-- Name: standardvariationforcourse; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.standardvariationforcourse (
    standardvariationforcourseid integer NOT NULL,
    standardvariationid integer NOT NULL,
    courseid integer NOT NULL
);


ALTER TABLE public.standardvariationforcourse OWNER TO postgres;

--
-- Name: standard_variation_for_course_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.standard_variation_for_course_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.standard_variation_for_course_id_seq OWNER TO postgres;

--
-- Name: standard_variation_for_course_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.standard_variation_for_course_id_seq OWNED BY public.standardvariationforcourse.standardvariationforcourseid;


--
-- Name: standardvariations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.standardvariations (
    standardvariationid integer NOT NULL,
    name character varying(49) NOT NULL
);


ALTER TABLE public.standardvariations OWNER TO postgres;

--
-- Name: standard_variation_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.standard_variation_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.standard_variation_id_seq OWNER TO postgres;

--
-- Name: standard_variation_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.standard_variation_id_seq OWNED BY public.standardvariations.standardvariationid;


--
-- Name: standardvariationitem; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.standardvariationitem (
    standardvariationitemid integer NOT NULL,
    ingredientid integer NOT NULL,
    tipovariazione character varying(30) NOT NULL,
    plailnumvariation integer,
    ingredientpriceid integer,
    standardvariationid integer NOT NULL
);


ALTER TABLE public.standardvariationitem OWNER TO postgres;

--
-- Name: standard_variation_item_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.standard_variation_item_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.standard_variation_item_id_seq OWNER TO postgres;

--
-- Name: standard_variation_item_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.standard_variation_item_id_seq OWNED BY public.standardvariationitem.standardvariationitemid;


--
-- Name: standardvariationforcoursedetails; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.standardvariationforcoursedetails AS
 SELECT a.standardvariationforcourseid,
    a.standardvariationid,
    a.courseid,
    b.name AS standardvariationname
   FROM (public.standardvariationforcourse a
     JOIN public.standardvariations b ON ((b.standardvariationid = a.standardvariationid)));


ALTER TABLE public.standardvariationforcoursedetails OWNER TO postgres;

--
-- Name: standardvariationitemdetails; Type: VIEW; Schema: public; Owner: postgres
--

CREATE VIEW public.standardvariationitemdetails AS
 SELECT a.standardvariationitemid,
    a.standardvariationid,
    a.ingredientid,
    a.tipovariazione,
    b.name AS ingredientname,
    b.allergen,
    a.plailnumvariation,
    a.ingredientpriceid,
    e.quantity,
    e.addprice,
    e.subtractprice
   FROM (((public.standardvariationitem a
     JOIN public.ingredient b ON ((a.ingredientid = b.ingredientid)))
     JOIN public.standardvariations c ON ((a.standardvariationid = c.standardvariationid)))
     LEFT JOIN public.ingredientprice e ON ((a.ingredientpriceid = e.ingredientpriceid)))
  ORDER BY b.name;


ALTER TABLE public.standardvariationitemdetails OWNER TO postgres;

--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.tempuseractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.tempuseractionablestates_seq OWNER TO postgres;

--
-- Name: temp_user_actionable_states; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.temp_user_actionable_states (
    tempuseractionablestateid integer DEFAULT nextval('public.tempuseractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.temp_user_actionable_states OWNER TO postgres;

--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.temp_user_actionable_states_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.temp_user_actionable_states_seq OWNER TO postgres;

--
-- Name: temp_user_default_actionable_states; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.temp_user_default_actionable_states (
    tempmuseractionablestatesid integer DEFAULT nextval('public.temp_user_actionable_states_seq'::regclass) NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.temp_user_default_actionable_states OWNER TO postgres;

--
-- Name: users_userid_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.users_userid_seq as integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.users_userid_seq OWNER TO postgres;

--
-- Name: users_userid_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.users_userid_seq OWNED BY public.users.userid;


--
-- Name: usersview; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.usersview OWNER TO postgres;

--
-- Name: variations; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.variations (
    variationsid integer NOT NULL,
    orderitemid integer NOT NULL,
    ingredientid integer NOT NULL,
    tipovariazione character varying(30) NOT NULL,
    plailnumvariation integer,
    ingredientpriceid integer
);


ALTER TABLE public.variations OWNER TO postgres;

--
-- Name: variationdetails; Type: VIEW; Schema: public; Owner: postgres
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


ALTER TABLE public.variationdetails OWNER TO postgres;

--
-- Name: variations_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.variations_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.variations_seq OWNER TO postgres;

--
-- Name: variations_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.variations_seq OWNED BY public.variations.variationsid;


--
-- Name: voidedorderslogbuffer; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.voidedorderslogbuffer (
    voidedorderslogbufferid integer NOT NULL,
    voidedtime timestamp without time zone NOT NULL,
    orderid integer NOT NULL,
    userid integer NOT NULL
);


ALTER TABLE public.voidedorderslogbuffer OWNER TO postgres;

--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.voidedorderslog_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.voidedorderslog_id_seq OWNER TO postgres;

--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.voidedorderslog_id_seq OWNED BY public.voidedorderslogbuffer.voidedorderslogbufferid;


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.waiteractionablestates_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER TABLE public.waiteractionablestates_seq OWNER TO postgres;

--
-- Name: waiteractionablestates; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.waiteractionablestates (
    waiterwatchablestatesid integer DEFAULT nextval('public.waiteractionablestates_seq'::regclass) NOT NULL,
    userid integer NOT NULL,
    stateid integer NOT NULL
);


ALTER TABLE public.waiteractionablestates OWNER TO postgres;

--
-- Name: archivedorderslogbuffer archivedlogbufferid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.archivedorderslogbuffer ALTER COLUMN archivedlogbufferid SET DEFAULT nextval('public.archivedorderslog_id_seq'::regclass);


--
-- Name: coursecategories categoryid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.coursecategories ALTER COLUMN categoryid SET DEFAULT nextval('public.courses_categoryid_seq'::regclass);


--
-- Name: courses courseid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.courses ALTER COLUMN courseid SET DEFAULT nextval('public.courses_courseid_seq'::regclass);


--
-- Name: ingredient ingredientid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient ALTER COLUMN ingredientid SET DEFAULT nextval('public.ingredientid_seq'::regclass);


--
-- Name: ingredientcategory ingredientcategoryid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcategory ALTER COLUMN ingredientcategoryid SET DEFAULT nextval('public.ingredient_categoryid_seq'::regclass);


--
-- Name: ingredientcourse ingredientcourseid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcourse ALTER COLUMN ingredientcourseid SET DEFAULT nextval('public.ingredientcourseid_seq'::regclass);


--
-- Name: orderitems orderitemid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems ALTER COLUMN orderitemid SET DEFAULT nextval('public.orderitems_orderitemid_seq'::regclass);


--
-- Name: orderitemstates orderitemstatesid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemstates ALTER COLUMN orderitemstatesid SET DEFAULT nextval('public.orderitemstates_orderitemstates_id_seq'::regclass);


--
-- Name: orders orderid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders ALTER COLUMN orderid SET DEFAULT nextval('public.orders_orderid_seq'::regclass);


--
-- Name: roles roleid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles ALTER COLUMN roleid SET DEFAULT nextval('public.roles_roleid_seq'::regclass);


--
-- Name: standardvariationforcourse standardvariationforcourseid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationforcourse ALTER COLUMN standardvariationforcourseid SET DEFAULT nextval('public.standard_variation_for_course_id_seq'::regclass);


--
-- Name: standardvariationitem standardvariationitemid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationitem ALTER COLUMN standardvariationitemid SET DEFAULT nextval('public.standard_variation_item_id_seq'::regclass);


--
-- Name: standardvariations standardvariationid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariations ALTER COLUMN standardvariationid SET DEFAULT nextval('public.standard_variation_id_seq'::regclass);


--
-- Name: users userid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users ALTER COLUMN userid SET DEFAULT nextval('public.users_userid_seq'::regclass);


--
-- Name: variations variationsid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations ALTER COLUMN variationsid SET DEFAULT nextval('public.variations_seq'::regclass);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.voidedorderslogbuffer ALTER COLUMN voidedorderslogbufferid SET DEFAULT nextval('public.voidedorderslog_id_seq'::regclass);


--
-- Data for Name: archivedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.archivedorderslogbuffer (archivedlogbufferid, archivedtime, orderid) FROM stdin;
295	2022-08-06 14:09:49.604744	668
296	2022-08-06 14:14:56.803481	669
297	2022-08-07 16:52:07.677418	671
298	2022-08-07 16:52:12.473671	672
301	2022-08-08 12:19:00.797629	675
302	2022-08-08 12:21:18.112356	676
303	2022-08-08 12:21:34.168961	677
305	2023-04-19 21:12:30.227988	678
306	2023-04-19 21:12:30.969267	679
307	2023-04-19 21:12:31.574166	684
308	2023-04-19 21:12:32.147475	685
309	2024-05-16 18:54:52.077972	693
310	2024-05-17 09:39:16.752586	683
311	2024-05-17 09:55:38.969307	686
312	2024-05-17 09:56:24.556851	692
313	2024-05-17 11:59:10.035059	694
314	2024-05-17 22:20:47.946978	680
315	2024-05-17 22:20:51.712411	687
316	2024-05-17 22:21:04.064205	691
322	2024-05-19 21:55:11.810624	699
323	2024-05-19 21:55:12.438586	700
324	2024-05-19 21:55:12.987575	701
325	2024-05-19 21:55:13.441826	702
326	2024-05-19 22:11:40.600081	705
329	2024-05-21 07:10:52.098853	703
330	2024-05-21 11:50:57.554784	704
331	2024-05-21 11:50:58.500005	706
332	2024-05-21 11:50:59.669635	707
333	2024-05-21 11:51:00.681346	708
334	2024-05-21 11:51:01.755495	709
\.


--
-- Data for Name: commentsforcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.commentsforcourse (commentsforcourseid, courseid, standardcommentid) FROM stdin;
4	537	6
5	539	6
16	548	17
17	548	18
18	553	6
19	553	9
20	540	11
22	548	19
23	540	17
24	549	11
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.coursecategories (categoryid, name, visibile, abstract) FROM stdin;
73	pranzo	t	t
83	pizze	t	f
70	secondi	t	f
82	toast	t	f
84	primi	t	f
75	panini	t	f
74	bevande	t	f
91	gelati	t	f
92	vini	t	f
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.courses (courseid, name, description, price, categoryid, visibility) FROM stdin;
538	pranzo		10.00	73	f
536	bistecca		10.00	70	t
539	cola		3.00	74	t
541	paninosottiletta		2.00	75	t
548	toast tradizionale		4.00	82	t
549	margherita		4.00	83	t
550	Coca Cola		3.00	74	t
552	birra heniken		5.00	74	t
553	cuba libre		8.00	74	t
554	ravioli alla caprese		10.00	84	t
555	Piadina		1.00	75	t
557	hot dog		5.00	75	t
558	panino sfizioso		6.00	75	t
559	ortolana		4.50	83	t
560	hamburger doppio		8.00	75	t
561	aglianico		7.00	92	t
562	arananciata		4.00	74	t
563	panino speciale		6.00	75	t
540	cheeseburger		8.00	75	t
551	Broccoli e Salsiccia	Buona	10.00	83	t
564	pizza regina	prosciutto e funghi bianca	9.00	83	t
565	cono		4.00	91	t
566	Coca cola		5.00	92	t
\.


--
-- Data for Name: customerdata; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.customerdata (customerdataid, data, name) FROM stdin;
\.


--
-- Data for Name: defaultactionablestates; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.defaultactionablestates (defaultactionablestatesid, stateid) FROM stdin;
\.


--
-- Data for Name: enablers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.enablers (enablersid, roleid, stateid, categoryid) FROM stdin;
205	29	2	83
206	29	6	83
207	36	2	83
208	36	6	83
209	37	1	74
210	37	2	74
211	37	6	74
212	1	2	74
213	1	2	91
214	1	2	83
215	1	2	82
216	1	6	74
\.


--
-- Data for Name: ingredient; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient (ingredientid, ingredientcategoryid, name, description, visibility, allergen, updateavailabilityflag, availablequantity, checkavailabilityflag, unitmeasure) FROM stdin;
180	56	green salad		t	f	f	0.00	f	gr
185	59	hamburger		t	f	f	0.00	f	unità
186	60	panino morbido		t	f	f	0.00	f	unità
188	62	salsa berlinese		t	t	f	0.00	f	gr
213	56	Broccoli		t	f	f	0.00	f	gr
214	107	merluzzo		t	f	f	0.00	f	gr
217	62	pomodoro		t	f	f	0.00	f	gr
218	59	salame		t	f	f	0.00	f	gr
219	56	Pomodori		t	f	f	0.00	f	gr
195	60	pan carré bianco		t	f	f	0.00	f	unità
220	59	waste		t	f	f	0.00	f	gr
221	62	senape		t	f	f	0.00	f	gr
222	60	baguette		t	f	f	0.00	f	unità
211	106	mozzarella		t	f	f	0.00	f	unità
212	59	Salsiccia		t	f	f	0.00	f	gr
224	59	prosciutto		t	f	f	0.00	f	gr
210	106	sottiletta		t	f	t	0.00	t	unità
225	56	funghi		t	f	f	0.00	f	gr
226	112	cioccolato		t	f	f	0.00	f	gr
227	112	cono		t	f	f	0.00	f	gr
\.


--
-- Data for Name: ingredientcategory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientcategory (ingredientcategoryid, name, description, visibility) FROM stdin;
56	vegetables		t
62	salse		t
107	pesce		t
106	formaggi		t
60	pane		t
59	carni		t
111	vini		t
112	gelati		t
\.


--
-- Data for Name: ingredientcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientcourse (ingredientcourseid, courseid, ingredientid, quantity) FROM stdin;
367	540	185	1.00
368	540	186	1.00
369	540	180	\N
386	548	210	2.00
388	548	195	2.00
389	551	212	\N
390	551	213	\N
395	557	220	1.00
396	557	186	1.00
397	557	221	\N
399	558	210	1.00
400	558	188	\N
401	548	224	\N
402	563	212	\N
403	563	211	1.00
404	563	180	\N
405	540	210	1.00
406	564	224	\N
407	564	225	\N
408	564	217	\N
409	564	211	1.00
410	555	212	1.00
411	550	222	1.00
\.


--
-- Data for Name: ingredientdecrement; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientdecrement (ingredientdecrementid, orderitemid, typeofdecrement, presumednormalquantity, recordedquantity, preparatorid, registrationtime, ingredientid) FROM stdin;
332	1608	NORMAL	1.00	\N	2	2022-10-16 11:29:15.113882	185
333	1608	NORMAL	1.00	\N	2	2022-10-16 11:29:15.113882	186
334	1609	NORMAL	2.00	\N	2	2022-10-16 11:29:15.135981	195
335	1617	NORMAL	1.00	\N	2	2023-02-05 00:21:30.472788	185
336	1617	NORMAL	1.00	\N	2	2023-02-05 00:21:30.472788	186
337	1617	gr	1.00	\N	2	2023-02-05 00:21:30.472788	213
338	1620	NORMAL	2.00	\N	2	2023-02-05 00:24:40.542965	195
351	1636	NORMAL	1.00	\N	2	2024-05-16 18:44:12.865445	185
352	1636	NORMAL	1.00	\N	2	2024-05-16 18:44:12.865445	186
353	1645	NORMAL	2.00	\N	2	2024-05-17 08:42:47.570219	195
354	1645	NORMAL	2.00	\N	2	2024-05-17 08:42:47.570219	210
355	1646	NORMAL	2.00	\N	2	2024-05-17 09:01:07.861963	195
356	1646	NORMAL	2.00	\N	2	2024-05-17 09:01:07.861963	210
361	1654	NORMAL	2.00	\N	2	2024-05-17 16:33:26.58179	195
362	1654	NORMAL	2.00	\N	2	2024-05-17 16:33:26.58179	210
367	1663	NORMAL	2.00	\N	2	2024-05-18 19:58:39.013852	195
368	1663	NORMAL	2.00	\N	2	2024-05-18 19:58:39.013852	210
369	1668	NORMAL	2.00	\N	2	2024-05-18 20:16:55.702267	195
370	1668	NORMAL	2.00	\N	2	2024-05-18 20:16:55.702267	210
371	1696	NORMAL	1.00	\N	2	2024-05-19 22:09:58.747293	210
372	1696	PER_PREZZO_INGREDIENTE	1.00	\N	2	2024-05-19 22:09:58.747293	211
375	1708	NORMAL	1.00	0.00	2	2024-05-20 09:48:03.285595	195
376	1708	PER_PREZZO_INGREDIENTE	50.00	0.00	2	2024-05-20 09:48:03.285595	212
377	1709	NORMAL	1.00	0.00	2	2024-05-20 09:48:03.285595	195
378	1709	PER_PREZZO_INGREDIENTE	50.00	0.00	2	2024-05-20 09:48:03.285595	212
383	1707	NORMAL	2.00	\N	2	2024-05-20 13:38:14.535695	195
328	1605	NORMAL	1.00	0.00	2	2022-08-08 10:19:21.501696	185
329	1605	NORMAL	1.00	0.00	2	2022-08-08 10:19:21.501696	186
330	1606	NORMAL	1.00	0.00	2	2022-08-08 10:19:21.501696	185
331	1606	NORMAL	1.00	0.00	2	2022-08-08 10:19:21.501696	186
384	1707	NORMAL	2.00	\N	2	2024-05-20 13:38:14.535695	210
385	1693	NORMAL	1.00	\N	2	2024-05-20 18:01:29.15103	186
386	1693	NORMAL	1.00	\N	2	2024-05-20 18:01:29.15103	220
387	1704	NORMAL	2.00	\N	2	2024-05-20 18:01:29.202277	195
388	1704	NORMAL	2.00	\N	2	2024-05-20 18:01:29.202277	210
389	1716	NORMAL	2.00	\N	2	2024-05-20 18:13:20.968055	195
390	1716	NORMAL	2.00	\N	2	2024-05-20 18:13:20.968055	210
391	1718	NORMAL	2.00	\N	2	2024-05-21 06:27:12.914425	195
392	1718	NORMAL	2.00	\N	2	2024-05-21 06:27:12.914425	210
393	1719	NORMAL	2.00	\N	2	2024-05-21 06:27:12.950526	195
394	1719	NORMAL	2.00	\N	2	2024-05-21 06:27:12.950526	210
395	1720	NORMAL	2.00	\N	2	2024-05-21 06:29:20.192957	195
396	1722	NORMAL	1.00	\N	2	2024-05-21 07:07:41.15335	185
397	1722	NORMAL	1.00	\N	2	2024-05-21 07:07:41.15335	186
398	1722	NORMAL	1.00	\N	2	2024-05-21 07:07:41.15335	210
399	1723	NORMAL	1.00	\N	2	2024-05-21 07:07:41.176021	185
400	1723	NORMAL	1.00	\N	2	2024-05-21 07:07:41.176021	186
401	1723	NORMAL	1.00	\N	2	2024-05-21 07:07:41.176021	210
402	1724	NORMAL	1.00	\N	2	2024-05-21 07:07:41.185283	185
403	1724	NORMAL	1.00	\N	2	2024-05-21 07:07:41.185283	186
404	1725	NORMAL	1.00	\N	2	2024-05-21 12:20:26.960096	222
\.


--
-- Data for Name: ingredientincrement; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientincrement (ingredientincrementid, ingredientid, comment, unitofmeasure, quantity, userid, registrationtime) FROM stdin;
38	210	primo carico del 02/02/2023	unità	3.00	2	2022-07-27 18:40:12.080542
39	210	fornitura di oggi	unità	1.00	2	2022-07-29 09:32:32.094511
40	210	fornitura di oggi	unità	1.00	2	2022-07-29 09:32:34.327915
41	210	nuovo inserimento 	unità	2.00	2	2022-07-29 09:38:01.935469
42	210	nuovo carico giorno 13	unità	2.00	2	2022-07-29 19:09:41.205873
43	210	ciao	unità	10.00	2	2022-08-04 10:55:40.023851
44	210	7	unità	30.00	2	2024-05-18 21:44:44.092563
\.


--
-- Data for Name: ingredientprice; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientprice (ingredientpriceid, ingredientid, quantity, isdefaultadd, isdefaultsubtract, addprice, subtractprice) FROM stdin;
108	185	1.00	t	t	4.00	4.00
110	186	1.00	t	t	1.00	1.00
128	210	1.00	t	t	1.00	1.00
132	218	1.00	f	f	1.00	1.00
133	222	1.00	f	f	1.00	1.00
135	211	1.00	t	t	2.00	2.00
136	212	50.00	t	t	2.00	2.00
137	212	100.00	f	f	3.00	3.00
\.


--
-- Data for Name: invoices; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.invoices (invoicesid, data, invoicenumber, customerdataid, date, suborderid, orderid) FROM stdin;
46	fattura: n. 1\n\ndata:17/05/2024 11:59:24\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 11.00\n\n\nimponibile: 10.00\n\nIVA 10 %: 1.00\n\n	1	\N	2024-05-17 11:59:24.102686	\N	694
47	fattura: n. 2\n\ndata:17/05/2024 22:20:47\n\ninserire qui tutti i dati dell'azienda\n\n\nTotale: 40.00\n\n\nimponibile: 36.36\n\nIVA 10 %: 3.64\n\n	2	\N	2024-05-17 22:20:47.893899	\N	680
\.


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
211	1	1	75
212	2	1	75
213	6	1	75
226	1	29	83
227	2	29	83
228	6	29	83
229	1	36	83
230	2	36	83
231	6	36	83
232	1	37	74
233	2	37	74
234	6	37	74
235	2	1	74
236	2	1	91
237	2	1	83
238	2	1	82
239	6	1	74
\.


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, printcount) FROM stdin;
1617	540	1	684		8.00	2	\N	2023-02-05 00:20:53.234752	\N	665	f	0
1603	539	1	677		3.00	2	\N	2022-08-08 10:13:53.151568	\N	656	f	0
1601	536	1	676		10.00	2	\N	2022-08-08 10:08:03.181455	\N	655	f	0
1602	539	1	676		3.00	2	\N	2022-08-08 10:08:09.444223	\N	655	f	0
1587	536	1	668		10.00	1	\N	2022-08-06 09:38:43.620569	\N	644	f	0
1604	541	1	676		2.00	6	\N	2022-08-08 10:19:51.804462	2022-08-08 10:19:56.068255	657	f	0
1605	540	1	676		8.00	2	\N	2022-08-08 10:35:38.987992	\N	655	f	1
1590	548	1	669		4.00	1	\N	2022-08-06 09:39:02.256107	\N	645	f	0
1606	540	1	676		8.00	2	\N	2022-08-08 10:35:39.14387	\N	655	f	1
1618	551	1	685		33.00	2	\N	2023-02-05 00:22:42.266794	\N	666	f	0
1619	549	1	685		4.00	2	\N	2023-02-05 00:22:54.668937	\N	666	f	0
1591	539	1	668		3.00	1	\N	2022-08-06 10:08:07.825991	\N	644	f	0
1620	548	1	685	, ben cotto	4.00	2	\N	2023-02-05 00:24:23.076268	\N	666	f	0
1621	541	1	686		2.00	1	\N	2023-04-19 21:03:05.715905	\N	667	f	0
1588	540	1	668		8.00	1	\N	2022-08-06 09:38:47.988373	\N	644	f	0
1622	551	1	686		33.00	1	\N	2023-04-19 21:03:16.050759	\N	668	f	0
1645	548	1	694	, ben cotto	4.00	2	\N	2024-05-17 08:42:39.046796	\N	679	f	0
1592	549	1	668		4.00	6	\N	2022-08-06 11:09:46.537258	2022-08-06 11:15:41.99865	644	f	0
1608	540	1	678		8.00	2	\N	2022-10-16 11:29:01.969282	\N	658	f	0
1609	548	1	678	, ben cotto	4.00	2	\N	2022-10-16 11:29:06.139956	\N	658	f	0
1593	549	1	669		4.00	6	\N	2022-08-06 14:14:22.698061	2022-08-06 14:14:47.509745	645	f	0
1610	550	1	679		3.00	6	\N	2022-10-16 11:32:24.864489	2023-04-19 21:11:15.423845	659	f	0
1616	550	1	684		3.00	6	\N	2023-02-05 00:20:48.729341	2023-04-19 21:11:16.75959	665	f	0
1611	551	1	680		33.00	1	\N	2022-12-06 22:12:18.035153	\N	661	f	0
1595	549	1	671		4.00	6	\N	2022-08-07 16:07:07.52815	2022-08-07 16:17:30.79362	650	f	0
1721	550	1	709		3.00	2	\N	2024-05-21 07:03:12.517192	\N	709	f	0
1596	549	1	672		4.00	6	\N	2022-08-07 16:18:10.221839	2022-08-07 16:18:21.563292	651	f	0
1607	550	1	678		3.00	6	\N	2022-10-16 11:28:58.075677	2023-04-19 21:11:17.862408	658	f	0
1646	548	1	694	, cottura normale	4.00	2	\N	2024-05-17 09:00:59.461014	\N	680	f	0
1623	552	1	686		5.00	6	\N	2023-04-19 21:13:27.566535	2023-04-19 21:14:15.668943	667	f	0
1599	549	1	675		4.00	6	\N	2022-08-07 17:30:41.156552	2022-08-07 20:09:20.686568	654	f	0
1647	550	1	692		3.00	2	\N	2024-05-17 09:03:11.75414	\N	681	f	0
1648	550	1	694		3.00	2	\N	2024-05-17 09:04:26.039907	\N	682	f	0
1666	555	1	701		1.00	2	\N	2024-05-18 20:16:46.0214	\N	695	f	0
1654	548	1	680		4.00	2	\N	2024-05-17 16:33:21.369193	\N	687	f	0
1626	553	1	687		7.00	6	\N	2023-04-19 21:42:48.623221	2023-04-19 21:47:59.220429	669	f	0
1627	553	1	687	, poco ghiaccio	8.00	6	\N	2023-04-19 21:46:24.765387	2023-04-19 21:48:00.257384	669	f	0
1629	550	1	686		3.00	1	\N	2024-05-16 18:04:36.634026	\N	667	f	0
1635	538	1	693	\N	10.00	6	\N	2024-05-16 18:43:05.68457	\N	675	f	1
1625	553	1	686		7.00	1	\N	2023-04-19 21:36:46.057401	\N	667	f	0
1630	550	1	691		3.00	2	\N	2024-05-16 18:12:00.227027	\N	672	f	0
1615	551	1	683		34.00	2	\N	2023-01-06 20:02:37.211976	\N	664	f	0
1636	540	1	683		8.00	2	\N	2024-05-16 18:43:54.242624	\N	664	f	0
1637	554	1	683		10.00	2	\N	2024-05-16 18:43:56.494975	\N	664	f	0
1638	536	1	683		10.00	2	\N	2024-05-16 18:43:59.361288	\N	664	f	0
1639	551	1	683		33.00	2	\N	2024-05-16 18:44:05.240345	\N	664	f	0
1640	538	1	693	\N	10.00	6	\N	2024-05-16 18:54:31.719056	\N	675	f	1
1655	551	1	680		33.00	2	\N	2024-05-17 22:06:43.829139	\N	688	f	0
1656	550	1	680		3.00	2	\N	2024-05-17 22:06:58.25536	\N	688	f	0
1667	554	1	701		10.00	2	\N	2024-05-18 20:16:48.435693	\N	695	f	0
1668	548	1	701		4.00	2	\N	2024-05-18 20:16:50.91139	\N	695	f	0
1669	536	1	702		10.00	2	\N	2024-05-18 20:31:44.169352	\N	696	f	0
1662	551	1	699		33.00	2	\N	2024-05-18 19:57:56.132439	\N	693	f	0
1663	548	1	699		4.00	2	\N	2024-05-18 19:57:59.425791	\N	693	f	0
1664	551	1	700		33.00	2	\N	2024-05-18 20:05:25.164219	\N	694	f	0
1665	554	1	700		10.00	2	\N	2024-05-18 20:05:29.097325	\N	694	f	0
1670	555	1	702		1.00	2	\N	2024-05-18 20:31:50.449799	\N	696	f	0
1671	550	1	702		3.00	2	\N	2024-05-18 20:31:55.116931	\N	696	f	0
1694	550	1	703		3.00	2	\N	2024-05-19 21:55:32.828456	\N	698	f	0
1688	548	1	699	, al sangue	4.00	1	\N	2024-05-18 21:37:33.773827	\N	697	t	0
1687	548	1	699	, al sangue	4.00	1	\N	2024-05-18 21:34:15.054104	\N	697	t	0
1689	548	1	699	, al sangue, ben cotto, al sangue	4.00	1	\N	2024-05-18 21:39:22.150864	\N	697	t	0
1690	548	1	699	, al sangue, al sangue, ben cotto, cottura normale	4.00	1	\N	2024-05-18 21:40:54.183273	\N	697	t	0
1691	548	1	699		4.00	1	\N	2024-05-18 21:42:03.255504	\N	697	t	0
1692	548	1	699	, ben cotto, al sangue	4.00	1	\N	2024-05-18 21:43:43.604324	\N	697	f	0
1697	555	1	705		1.00	2	\N	2024-05-19 22:09:45.136164	\N	699	f	0
1696	558	1	705		7.00	2	\N	2024-05-19 22:05:52.045493	\N	699	f	0
1698	554	1	705		10.00	2	\N	2024-05-19 22:09:49.806798	\N	699	f	0
1699	550	1	705		3.00	2	\N	2024-05-19 22:09:54.957229	\N	699	f	0
1693	557	1	703		5.00	2	\N	2024-05-19 19:46:32.821218	\N	698	f	0
1695	551	1	703		33.00	2	\N	2024-05-19 21:55:36.171496	\N	698	f	0
1702	554	1	706		10.00	2	\N	2024-05-20 09:47:34.640687	\N	700	f	0
1703	549	1	706		4.00	2	\N	2024-05-20 09:47:40.152496	\N	700	f	0
1704	548	1	703		4.00	2	\N	2024-05-20 10:16:04.486277	\N	698	f	0
1708	548	1	706		4.00	2	\N	2024-05-20 13:36:34.511027	\N	702	f	1
1709	548	1	706		4.00	2	\N	2024-05-20 13:36:34.524754	\N	702	f	1
1711	555	1	706		1.00	2	\N	2024-05-20 13:37:11.323326	\N	702	f	1
1707	548	1	704	, al sangue, ben cotto	4.00	2	\N	2024-05-20 10:28:58.633726	\N	701	f	0
1712	559	1	704		4.50	2	\N	2024-05-20 14:00:30.362188	\N	703	f	0
1713	538	1	704	\N	10.00	6	\N	2024-05-20 14:08:53.747856	\N	704	f	1
1714	551	1	704		33.00	1	\N	2024-05-20 15:46:04.77867	\N	705	f	0
1715	551	1	707		33.00	2	\N	2024-05-20 18:12:42.800045	\N	706	f	0
1716	548	1	707		4.00	2	\N	2024-05-20 18:12:46.621153	\N	706	f	0
1717	550	1	707		3.00	2	\N	2024-05-20 18:12:56.503908	\N	706	f	0
1723	540	1	709		8.00	6	\N	2024-05-21 07:04:34.820642	2024-05-21 08:44:16.04924	709	f	0
1718	548	1	708		4.00	2	\N	2024-05-21 06:23:23.769601	\N	707	f	0
1719	548	1	708	, piccante	4.00	2	\N	2024-05-21 06:25:42.10112	\N	707	f	0
1722	540	1	709	, ben cotto	8.00	6	\N	2024-05-21 07:03:25.864862	2024-05-21 08:44:17.263021	709	f	0
1724	540	1	709	, al sangue	9.00	6	\N	2024-05-21 07:06:17.268249	2024-05-21 08:44:18.398532	709	f	0
1720	548	1	708	, ben cotto	5.00	2	\N	2024-05-21 06:29:03.118923	\N	708	f	0
1725	550	1	710		3.00	6	\N	2024-05-21 12:20:24.107254	2024-05-21 12:54:52.058059	710	f	0
\.


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
3034	1662	2	2024-05-18 19:58:38.965178
3035	1663	2	2024-05-18 19:58:39.011172
3040	1666	1	2024-05-18 20:16:46.0214
3041	1667	1	2024-05-18 20:16:48.435693
3042	1668	1	2024-05-18 20:16:50.91139
3043	1666	2	2024-05-18 20:16:55.653172
3044	1667	2	2024-05-18 20:16:55.688494
3045	1668	2	2024-05-18 20:16:55.698456
3067	1687	1	2024-05-18 21:34:15.054104
3069	1689	1	2024-05-18 21:39:22.150864
3071	1691	1	2024-05-18 21:42:03.255504
3073	1693	1	2024-05-19 19:46:32.821218
3076	1696	1	2024-05-19 22:05:52.045493
3092	1704	1	2024-05-20 10:16:04.486277
3096	1709	1	2024-05-20 09:43:19.137783
3097	1708	2	2024-05-20 09:48:03.275568
3098	1708	1	2024-05-20 09:43:19.137783
3099	1709	2	2024-05-20 09:48:03.275568
3100	1711	1	2024-05-20 09:47:27.88445
3101	1711	2	2024-05-20 09:48:03.346267
3106	1707	2	2024-05-20 13:38:14.534377
3109	1714	1	2024-05-20 15:46:04.77867
3114	1715	1	2024-05-20 18:12:42.800045
2889	1587	1	2022-08-06 09:38:43.620569
2890	1588	1	2022-08-06 09:38:47.988373
3115	1716	1	2024-05-20 18:12:46.621153
2892	1590	1	2022-08-06 09:39:02.256107
2893	1591	1	2022-08-06 10:08:07.825991
2894	1592	1	2022-08-06 11:09:46.537258
2895	1592	2	2022-08-06 11:15:11.260055
2896	1592	6	2022-08-06 11:15:41.99865
2897	1593	1	2022-08-06 14:14:22.698061
2898	1593	2	2022-08-06 14:14:30.286965
2899	1593	6	2022-08-06 14:14:47.509745
3116	1717	1	2024-05-20 18:12:56.503908
3117	1715	2	2024-05-20 18:13:20.939549
3118	1716	2	2024-05-20 18:13:20.965991
2903	1595	1	2022-08-07 16:07:07.52815
2904	1595	2	2022-08-07 16:17:27.086737
2905	1595	6	2022-08-07 16:17:30.79362
2906	1596	1	2022-08-07 16:18:10.221839
2907	1596	2	2022-08-07 16:18:18.534015
2908	1596	6	2022-08-07 16:18:21.563292
3119	1717	2	2024-05-20 18:13:21.022019
3124	1720	1	2024-05-21 06:29:03.118923
3125	1720	2	2024-05-21 06:29:20.188606
3128	1723	1	2024-05-21 07:04:34.820642
3129	1724	1	2024-05-21 07:06:17.268249
2912	1599	1	2022-08-07 17:30:41.156552
2913	1599	2	2022-08-07 20:09:08.223475
2914	1599	6	2022-08-07 20:09:20.686568
3130	1721	2	2024-05-21 07:07:41.124085
2916	1601	1	2022-08-08 10:08:03.181455
2917	1602	1	2022-08-08 10:08:09.444223
2918	1603	1	2022-08-08 10:13:53.151568
2919	1603	2	2022-08-08 10:19:13.165534
3131	1722	2	2024-05-21 07:07:41.15038
2921	1601	2	2022-08-08 10:19:21.509699
2922	1602	2	2022-08-08 10:19:21.517008
2923	1604	1	2022-08-08 10:19:51.804462
2924	1604	2	2022-08-08 10:19:55.877531
2925	1604	6	2022-08-08 10:19:56.068255
2926	1606	1	2022-08-08 10:07:52.352625
2927	1606	2	2022-08-08 10:19:21.493227
2928	1605	2	2022-08-08 10:19:21.493227
2929	1605	1	2022-08-08 10:07:52.352625
2930	1607	1	2022-10-16 11:28:58.075677
2931	1608	1	2022-10-16 11:29:01.969282
2932	1609	1	2022-10-16 11:29:06.139956
2933	1607	2	2022-10-16 11:29:15.084581
2934	1608	2	2022-10-16 11:29:15.110888
2935	1609	2	2022-10-16 11:29:15.134215
2936	1610	1	2022-10-16 11:32:24.864489
2937	1610	2	2022-10-16 11:32:28.057194
2938	1611	1	2022-12-06 22:12:18.035153
3132	1723	2	2024-05-21 07:07:41.174175
3133	1724	2	2024-05-21 07:07:41.182939
3138	1725	1	2024-05-21 12:20:24.107254
2942	1615	1	2023-01-06 20:02:37.211976
2943	1616	1	2023-02-05 00:20:48.729341
2944	1617	1	2023-02-05 00:20:53.234752
2945	1616	2	2023-02-05 00:21:30.436906
2946	1617	2	2023-02-05 00:21:30.468834
2947	1618	1	2023-02-05 00:22:42.266794
2948	1619	1	2023-02-05 00:22:54.668937
2949	1620	1	2023-02-05 00:24:23.076268
2950	1618	2	2023-02-05 00:24:40.488675
2951	1619	2	2023-02-05 00:24:40.508763
2952	1620	2	2023-02-05 00:24:40.536152
2953	1621	1	2023-04-19 21:03:05.715905
2954	1622	1	2023-04-19 21:03:16.050759
2955	1610	6	2023-04-19 21:11:15.423845
2956	1616	6	2023-04-19 21:11:16.75959
2957	1607	6	2023-04-19 21:11:17.862408
2958	1623	1	2023-04-19 21:13:27.566535
2959	1623	2	2023-04-19 21:13:52.101388
2960	1623	6	2023-04-19 21:14:15.668943
3139	1725	2	2024-05-21 12:20:26.932298
2964	1625	1	2023-04-19 21:36:46.057401
2965	1626	1	2023-04-19 21:42:48.623221
2966	1627	1	2023-04-19 21:46:24.765387
2967	1626	2	2023-04-19 21:47:33.67543
2968	1627	2	2023-04-19 21:47:33.688109
2969	1626	6	2023-04-19 21:47:59.220429
2970	1627	6	2023-04-19 21:48:00.257384
2972	1629	1	2024-05-16 18:04:36.634026
2973	1630	1	2024-05-16 18:12:00.227027
2974	1630	2	2024-05-16 18:12:09.65382
2983	1636	1	2024-05-16 18:43:54.242624
2984	1637	1	2024-05-16 18:43:56.494975
2985	1638	1	2024-05-16 18:43:59.361288
2986	1639	1	2024-05-16 18:44:05.240345
2987	1615	2	2024-05-16 18:44:12.854574
2988	1636	2	2024-05-16 18:44:12.863265
2989	1637	2	2024-05-16 18:44:12.875093
2990	1638	2	2024-05-16 18:44:12.880842
2991	1639	2	2024-05-16 18:44:12.886834
2998	1645	1	2024-05-17 08:42:39.046796
2999	1645	2	2024-05-17 08:42:47.562766
3000	1646	1	2024-05-17 09:00:59.461014
3001	1646	2	2024-05-17 09:01:07.854866
3002	1647	1	2024-05-17 09:03:11.75414
3003	1647	2	2024-05-17 09:03:13.760701
3004	1648	1	2024-05-17 09:04:26.039907
3005	1648	2	2024-05-17 09:04:27.949803
3036	1664	1	2024-05-18 20:05:25.164219
3037	1665	1	2024-05-18 20:05:29.097325
3016	1654	1	2024-05-17 16:33:21.369193
3017	1654	2	2024-05-17 16:33:26.574895
3018	1655	1	2024-05-17 22:06:43.829139
3019	1656	1	2024-05-17 22:06:58.25536
3020	1655	2	2024-05-17 22:07:00.863047
3021	1656	2	2024-05-17 22:07:00.947952
3038	1664	2	2024-05-18 20:05:55.393134
3039	1665	2	2024-05-18 20:05:55.49459
3046	1669	1	2024-05-18 20:31:44.169352
3047	1670	1	2024-05-18 20:31:50.449799
3048	1671	1	2024-05-18 20:31:55.116931
3049	1669	2	2024-05-18 20:31:56.685034
3050	1670	2	2024-05-18 20:31:56.751468
3051	1671	2	2024-05-18 20:31:56.765373
3032	1662	1	2024-05-18 19:57:56.132439
3033	1663	1	2024-05-18 19:57:59.425791
3068	1688	1	2024-05-18 21:37:33.773827
3070	1690	1	2024-05-18 21:40:54.183273
3072	1692	1	2024-05-18 21:43:43.604324
3074	1694	1	2024-05-19 21:55:32.828456
3075	1695	1	2024-05-19 21:55:36.171496
3077	1697	1	2024-05-19 22:09:45.136164
3078	1698	1	2024-05-19 22:09:49.806798
3079	1699	1	2024-05-19 22:09:54.957229
3080	1696	2	2024-05-19 22:09:58.739276
3081	1697	2	2024-05-19 22:09:58.795624
3082	1698	2	2024-05-19 22:09:58.801357
3083	1699	2	2024-05-19 22:09:58.805712
3086	1702	1	2024-05-20 09:47:34.640687
3087	1703	1	2024-05-20 09:47:40.152496
3090	1702	2	2024-05-20 09:48:03.43043
3091	1703	2	2024-05-20 09:48:03.439282
3095	1707	1	2024-05-20 10:28:58.633726
3107	1712	1	2024-05-20 14:00:30.362188
3108	1712	2	2024-05-20 14:00:32.501213
3110	1693	2	2024-05-20 18:01:29.112745
3111	1694	2	2024-05-20 18:01:29.191513
3112	1695	2	2024-05-20 18:01:29.196281
3113	1704	2	2024-05-20 18:01:29.200801
3120	1718	1	2024-05-21 06:23:23.769601
3121	1719	1	2024-05-21 06:25:42.10112
3122	1718	2	2024-05-21 06:27:12.907882
3123	1719	2	2024-05-21 06:27:12.948948
3126	1721	1	2024-05-21 07:03:12.517192
3127	1722	1	2024-05-21 07:03:25.864862
3135	1723	6	2024-05-21 08:44:16.04924
3136	1722	6	2024-05-21 08:44:17.263021
3137	1724	6	2024-05-21 08:44:18.398532
3140	1725	6	2024-05-21 12:54:52.058059
\.


--
-- Data for Name: orderitemsubordermapping; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitemsubordermapping (orderitemsubordermappingid, orderitemid, suborderid) FROM stdin;
146	1696	428
147	1697	428
148	1698	429
149	1699	430
162	1708	438
163	1709	438
103	1601	408
104	1602	408
105	1604	409
106	1605	409
107	1606	410
164	1711	438
165	1703	439
166	1702	440
167	1693	441
168	1694	441
169	1695	442
114	1608	415
115	1609	415
170	1704	443
\.


--
-- Data for Name: orderoutgroup; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderoutgroup (ordergroupid, printcount, orderid, groupidentifier) FROM stdin;
694	1	700	1
693	2	699	1
695	1	701	1
696	2	702	1
697	0	699	2
699	1	705	1
702	0	706	2
644	0	668	1
645	0	669	1
701	1	704	1
650	0	671	1
651	0	672	1
704	1	704	99
703	2	704	2
705	0	704	3
654	1	675	1
698	1	703	1
656	2	677	1
655	1	676	1
700	2	706	1
657	2	676	2
658	1	678	1
659	1	679	1
665	1	684	1
706	4	707	1
666	1	685	1
667	0	686	1
668	0	686	2
669	1	687	1
707	1	708	1
672	1	691	4
673	1	693	1
674	1	693	2
664	1	683	1
675	1	693	99
708	1	708	2
709	1	709	1
679	3	694	1
680	1	694	2
681	1	692	1
710	1	710	1
682	5	694	3
661	1	680	1
687	1	680	2
688	1	680	3
\.


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
693	13		t	2	2024-05-16 18:39:57.577316	2024-05-16 18:54:52.07626	f	t	20.00	20.00	0.00	0.00	f	f	\N
671	3		f	2	2022-08-07 16:07:03.782061	2022-08-07 16:52:07.672536	f	t	4.00	4.00	0.00	0.00	f	f	\N
683	12		t	2	2023-01-06 19:58:39.780131	2024-05-17 09:39:16.748499	f	t	95.00	95.00	0.00	0.00	f	f	\N
680	5		t	2	2022-12-06 22:12:05.144251	2024-05-17 22:20:47.945565	f	t	40.00	40.00	0.00	0.00	f	f	\N
672	1		f	2	2022-08-07 16:18:01.081507	2022-08-07 16:52:12.472289	f	t	4.00	4.00	0.00	0.00	f	f	\N
699	1		t	2	2024-05-18 19:57:52.186452	2024-05-19 21:55:11.809731	f	t	37.00	37.00	0.00	0.00	f	f	\N
675	1		f	2	2022-08-07 17:30:38.231263	2022-08-08 12:19:00.796388	f	t	4.00	4.00	0.00	0.00	f	f	\N
687	100		f	2	2023-04-19 21:42:40.391221	2024-05-17 22:20:51.710928	f	t	15.00	15.00	0.00	0.00	f	f	\N
676	2		t	2	2022-08-08 10:07:48.326193	2022-08-08 12:21:18.066067	f	t	31.00	31.00	0.00	0.00	f	f	\N
677	3		t	2	2022-08-08 10:13:42.766262	2022-08-08 12:21:34.168113	f	t	3.00	3.00	0.00	0.00	f	f	\N
668	1		t	2	2022-08-06 09:30:18.605404	2022-08-06 14:09:49.603436	f	t	4.00	4.00	0.00	0.00	f	f	\N
669	2		t	2	2022-08-06 09:38:53.022116	2022-08-06 14:14:56.801596	f	t	4.00	4.00	0.00	0.00	f	f	\N
700	2		t	2	2024-05-18 20:05:21.014627	2024-05-19 21:55:12.437934	f	t	43.00	43.00	0.00	0.00	f	f	\N
704	19		t	198	2024-05-19 19:47:36.170391	2024-05-21 11:50:57.547639	f	t	18.50	18.50	0.00	0.00	f	f	\N
701	3		t	2	2024-05-18 20:16:42.534045	2024-05-19 21:55:12.986495	f	t	15.00	15.00	0.00	0.00	f	f	\N
702	6		t	2	2024-05-18 20:30:33.717445	2024-05-19 21:55:13.441087	f	t	14.00	14.00	0.00	0.00	f	f	\N
678	4		t	2	2022-10-16 11:28:54.48376	2023-04-19 21:12:30.226373	f	t	15.00	15.00	0.00	0.00	f	f	\N
694	d		t	195	2024-05-17 07:13:15.017823	2024-05-17 11:59:10.030754	f	t	11.00	11.00	0.00	0.00	f	f	\N
691	10		t	2	2024-05-16 18:03:57.849089	2024-05-17 22:21:04.062508	f	t	3.00	3.00	0.00	0.00	f	f	\N
679	3		f	2	2022-10-16 11:31:43.300308	2023-04-19 21:12:30.967352	f	t	3.00	3.00	0.00	0.00	f	f	\N
706	1		t	2	2024-05-20 09:43:05.252915	2024-05-21 11:50:58.499325	f	t	23.00	23.00	0.00	0.00	f	f	\N
684	6		t	2	2023-02-05 00:20:44.787614	2023-04-19 21:12:31.573026	f	t	11.00	11.00	0.00	0.00	f	f	\N
686	99		t	2	2023-04-19 20:24:26.540813	2024-05-17 09:55:38.965458	f	t	5.00	5.00	0.00	0.00	f	f	\N
685	67		t	2	2023-02-05 00:22:20.865285	2023-04-19 21:12:32.146116	f	t	41.00	41.00	0.00	0.00	f	f	\N
707	3		t	2	2024-05-20 18:12:33.802779	2024-05-21 11:50:59.668236	f	t	40.00	40.00	0.00	0.00	f	f	\N
703	10		t	2	2024-05-19 19:46:25.600722	2024-05-21 07:10:52.09789	f	t	45.00	45.00	0.00	0.00	f	f	\N
708	8		t	2	2024-05-21 06:23:09.528493	2024-05-21 11:51:00.68045	f	t	13.00	13.00	0.00	0.00	f	f	\N
709	11		t	2	2024-05-21 07:02:55.761937	2024-05-21 11:51:01.754216	f	t	28.00	28.00	0.00	0.00	f	f	\N
705	7		t	2	2024-05-19 22:05:42.132749	2024-05-19 22:11:40.59722	f	t	21.00	21.00	0.00	0.00	f	f	\N
692	8		t	194	2024-05-16 18:15:49.018972	2024-05-17 09:56:24.552207	f	t	3.00	3.00	0.00	0.00	f	f	\N
710	1		f	2	2024-05-21 12:20:16.508335	\N	f	f	3.00	3.00	0.00	0.00	f	f	\N
\.


--
-- Data for Name: paymentitem; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.paymentitem (paymentid, suborderid, orderid, tendercodesid, amount) FROM stdin;
155	\N	668	1	4.00
156	\N	669	1	4.00
157	\N	671	1	4.00
158	\N	672	1	4.00
161	\N	675	1	4.00
162	408	\N	1	5.00
163	408	\N	3	5.00
164	408	\N	6	3.00
165	409	\N	1	10.00
166	410	\N	1	8.00
167	\N	677	1	3.00
170	415	\N	1	12.00
171	\N	693	1	20.00
173	\N	683	1	95.00
174	\N	694	1	11.00
175	\N	686	1	5.00
176	\N	692	1	3.00
177	\N	687	1	15.00
178	\N	680	1	40.00
179	\N	691	1	3.00
180	428	\N	1	8.00
181	429	\N	1	5.00
182	429	\N	3	5.00
183	430	\N	6	3.00
189	\N	706	1	23.00
190	\N	704	1	18.50
191	438	\N	1	9.00
192	441	\N	1	8.00
193	442	\N	1	33.00
194	443	\N	1	4.00
\.


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
66	82	50	2
67	75	50	2
68	70	50	2
76	73	50	2
77	83	50	2
86	73	52	2
89	70	52	2
91	84	52	2
92	91	52	2
94	74	52	2
95	82	52	2
96	75	52	2
\.


--
-- Data for Name: printerforreceiptandinvoice; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.printerforreceiptandinvoice (printerforcategoryid, printinvoice, printreceipt, printerid) FROM stdin;
\.


--
-- Data for Name: printers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.printers (printerid, name) FROM stdin;
50	PDF_Printer
52	HPC01803AC853C__HP_Color_Laser_150_
\.


--
-- Data for Name: rejectedorderitems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rejectedorderitems (rejectedorderitemid, courseid, cause, timeofrejection, orderitemid) FROM stdin;
93	548	mancano: sottiletta	2024-05-17 16:33:21.384239	1654
96	548	mancano: sottiletta	2024-05-18 19:57:59.442691	1663
97	548	mancano: sottiletta	2024-05-18 20:16:50.934465	1668
113	548	mancano: sottiletta	2024-05-18 21:34:15.143207	1687
114	548	mancano: sottiletta	2024-05-18 21:37:33.855728	1688
115	548	mancano: sottiletta	2024-05-18 21:39:22.234058	1689
116	548	mancano: sottiletta	2024-05-18 21:40:54.252312	1690
117	548	mancano: sottiletta	2024-05-18 21:42:03.343921	1691
118	548	mancano: sottiletta	2024-05-18 21:43:43.684876	1692
\.


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (roleid, rolename, comment) FROM stdin;
1	admin	\N
28	temporary	
29	cameriere	
36	pizzaiolo	
37	barman	
\.


--
-- Data for Name: standardcomments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardcomments (standardcommentid, comment) FROM stdin;
6	poco ghiaccio
9	molto ghiaccio
11	al sangue
17	ben cotto
18	cottura normale
19	piccante
\.


--
-- Data for Name: standardvariationforcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariationforcourse (standardvariationforcourseid, standardvariationid, courseid) FROM stdin;
8	10	548
9	11	553
10	10	540
\.


--
-- Data for Name: standardvariationitem; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariationitem (standardvariationitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid, standardvariationid) FROM stdin;
5	182	👍	\N	\N	4
16	188	👍	\N	\N	6
17	180	🚫	\N	\N	6
24	211	👍	\N	\N	10
25	210	🚫	\N	128	10
29	213	😍	\N	\N	11
26	216	👍	\N	\N	11
\.


--
-- Data for Name: standardvariations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariations (standardvariationid, name) FROM stdin;
4	havana
6	variante berlinese
10	con mozzarella
11	con havana 5
12	piccante
\.


--
-- Data for Name: states; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.states (stateid, isinitial, isfinal, statusname, nextstateid, isexceptional, creatingingredientdecrement) FROM stdin;
1	t	f	COLLECTING	2	f	f
6	f	t	DONE	\N	f	f
2	f	f	TOBEWORKED	6	f	t
\.


--
-- Data for Name: subcategorymapping; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.subcategorymapping (subcategorymappingid, fatherid, sonid) FROM stdin;
\.


--
-- Data for Name: suborder; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suborder (suborderid, orderid, subtotal, comment, payed, creationtime, tendercodesid, subtotaladjustment, subtotalpercentadjustment) FROM stdin;
408	676	13.00	\N	t	2022-08-08 12:19:16.221	\N	0.00	0.00
409	676	10.00	\N	t	2022-08-08 12:19:25.106753	\N	0.00	0.00
410	676	8.00	\N	t	2022-08-08 12:19:42.931406	\N	0.00	0.00
441	703	8.00	\N	t	2024-05-21 07:09:43.708662	\N	0.00	0.00
442	703	33.00	\N	t	2024-05-21 07:09:47.048902	\N	0.00	0.00
428	705	8.00	\N	t	2024-05-19 22:10:28.454592	\N	0.00	0.00
429	705	10.00	\N	t	2024-05-19 22:10:34.228489	\N	0.00	0.00
430	705	3.00	\N	t	2024-05-19 22:10:36.761237	\N	0.00	0.00
443	703	4.00	\N	t	2024-05-21 07:09:49.557145	\N	0.00	0.00
415	678	12.00	\N	t	2023-01-06 20:18:30.520686	\N	0.00	0.00
439	706	4.00	\N	f	2024-05-21 06:31:21.925464	\N	0.00	0.00
440	706	10.00	\N	f	2024-05-21 06:31:28.698544	\N	0.00	0.00
438	706	9.00	\N	t	2024-05-21 06:31:01.258388	\N	0.00	0.00
\.


--
-- Data for Name: temp_user_actionable_states; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temp_user_actionable_states (tempuseractionablestateid, userid, stateid) FROM stdin;
\.


--
-- Data for Name: temp_user_default_actionable_states; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.temp_user_default_actionable_states (tempmuseractionablestatesid, stateid) FROM stdin;
\.


--
-- Data for Name: tendercodes; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.tendercodes (tendercodesid, tendercode, tendername) FROM stdin;
1	1	CONTANTI
2	2	CREDITO
3	3	ASSEGNI
5	4	BUONI
6	5	CARTA DI CREDITO
\.


--
-- Data for Name: users; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.users (userid, username, password, enabled, canvoidorders, role, canmanageallorders, creationtime, istemporary, canchangetheprice, "table", consumed, canmanagecourses) FROM stdin;
2	administrator	4194d1706ed1f408d5e02d672777019f4d5385c766a8c6ca8acba3167d36a7b9	t	t	1	t	\N	f	t	\N	\N	f
183	tonino	87cfad1b5f9782a36c7b8fd877c65a6f7d96e6499149323826ad4bc327bbf37c	t	f	28	t	\N	f	f	\N	\N	f
191	giorgio	ecb38115d0b47f3584a9d632cc6660f2004145be05bc8a24e4e6ccb2d15cf64b	t	f	36	f	\N	f	f	\N	\N	f
192	X2NK1IDD963A		t	f	28	f	2022-12-06 22:14:47.015809	t	f	7	\N	f
193	benny	3fe289d7d1bf66596b7ccc72dd8b03bd664c7ae74f9090979638f12a8c0daa31	t	f	37	t	\N	f	t	\N	\N	t
194	RG5STM3W3P5Z		t	f	28	f	2024-05-16 18:15:49.017181	t	f	8	\N	f
195	M147UTE92SD0		t	f	28	f	2024-05-17 07:13:15.002718	t	f	d	\N	f
196	cassiere	f2f017dd566e79b2b8f2c9ab0f2e7f125621a8c374142687fccbfe8db2b80116	t	f	28	f	\N	f	f	\N	\N	f
197	JPENFTOGYB15		t	f	28	f	2024-05-17 07:52:47.204444	t	f	9	\N	f
198	XB56BZPVW9XV		t	f	28	f	2024-05-19 19:47:36.162712	t	f	19	\N	f
\.


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
1197	1609	211	👍	\N	\N
1198	1609	210	🚫	\N	128
1199	1611	213	🚫	\N	\N
1202	1617	213	gr	1	\N
1203	1620	211	👍	\N	\N
1204	1620	210	🚫	\N	128
1216	1615	213	🚫	\N	\N
1217	1693	221	🚫	\N	\N
1218	1696	210	unità	-1	\N
1221	1696	211	PER_PREZZO_INGREDIENTE	\N	135
1222	1696	188	🚫	\N	\N
1228	1709	211	👍	\N	\N
1229	1708	211	👍	\N	\N
1230	1708	212	PER_PREZZO_INGREDIENTE	\N	\N
1231	1709	210	🚫	\N	\N
1232	1709	212	PER_PREZZO_INGREDIENTE	\N	\N
1233	1708	210	🚫	\N	\N
1234	1720	211	👍	\N	\N
1235	1720	210	🚫	\N	128
1236	1720	219	👍	\N	\N
1237	1724	211	👍	\N	\N
1238	1724	210	🚫	\N	128
1239	1724	180	🚫	\N	\N
\.


--
-- Data for Name: voidedorderslogbuffer; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.voidedorderslogbuffer (voidedorderslogbufferid, voidedtime, orderid, userid) FROM stdin;
\.


--
-- Data for Name: waiteractionablestates; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.waiteractionablestates (waiterwatchablestatesid, userid, stateid) FROM stdin;
301	2	1
302	2	2
\.


--
-- Name: archivedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 334, true);


--
-- Name: comments_for_course_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.comments_for_course_seq', 24, true);


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 92, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 566, true);


--
-- Name: customerdata_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.customerdata_id_seq', 12, true);


--
-- Name: defaulwaiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.defaulwaiteractionablestates_seq', 30, true);


--
-- Name: enablers_elablersid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 216, true);


--
-- Name: incredientdecrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.incredientdecrementid_seq', 404, true);


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_categoryid_seq', 112, true);


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 411, true);


--
-- Name: ingredientid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientid_seq', 227, true);


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientincrementid_seq', 44, true);


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 139, true);


--
-- Name: invoicesid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.invoicesid_seq', 47, true);


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 239, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Name: orderitem_sub_order_mapping_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitem_sub_order_mapping_seq', 170, true);


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1725, true);


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 3140, true);


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 710, true);


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 710, true);


--
-- Name: paymentid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.paymentid_seq', 194, true);


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 96, true);


--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printerforreceiptandinvoice_id_seq', 5, true);


--
-- Name: printers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printers_id_seq', 52, true);


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 118, true);


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 37, true);


--
-- Name: standard_comments_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_comments_seq', 19, true);


--
-- Name: standard_variation_for_course_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_for_course_id_seq', 11, true);


--
-- Name: standard_variation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_id_seq', 12, true);


--
-- Name: standard_variation_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_item_id_seq', 31, true);


--
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.states_stateid_seq', 6, true);


--
-- Name: subcategory_mapping_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.subcategory_mapping_seq', 2, true);


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suborderid_seq', 443, true);


--
-- Name: temp_user_actionable_states_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.temp_user_actionable_states_seq', 12, true);


--
-- Name: tempuseractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tempuseractionablestates_seq', 1, false);


--
-- Name: tendercodesid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.tendercodesid_seq', 6, true);


--
-- Name: users_userid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.users_userid_seq', 198, true);


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.variations_seq', 1239, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 363, true);


--
-- Name: waiteractionablestates_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.waiteractionablestates_seq', 305, true);


--
-- Name: archivedorderslogbuffer archivedlogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT archivedlogbufferid_key PRIMARY KEY (archivedlogbufferid);


--
-- Name: coursecategories category_uniquename; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT category_uniquename UNIQUE (name);


--
-- Name: standardcomments comment_uniquename; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardcomments
    ADD CONSTRAINT comment_uniquename UNIQUE (comment);


--
-- Name: commentsforcourse commentsforcourse_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT commentsforcourse_pkey PRIMARY KEY (commentsforcourseid);


--
-- Name: coursecategories coursecategories_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.coursecategories
    ADD CONSTRAINT coursecategories_pkey PRIMARY KEY (categoryid);


--
-- Name: commentsforcourse coursecommentid_pair; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT coursecommentid_pair UNIQUE (courseid, standardcommentid);


--
-- Name: courses courses_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT courses_pkey PRIMARY KEY (courseid);


--
-- Name: customerdata customer_name_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customer_name_unique UNIQUE (name);


--
-- Name: customerdata customerdata_name_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_name_key UNIQUE (name);


--
-- Name: customerdata customerdata_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.customerdata
    ADD CONSTRAINT customerdata_pkey PRIMARY KEY (customerdataid);


--
-- Name: defaultactionablestates defaultactionablestateidunique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestateidunique UNIQUE (stateid);


--
-- Name: defaultactionablestates defaultactionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_pkey PRIMARY KEY (defaultactionablestatesid);


--
-- Name: enablers enablers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_pkey PRIMARY KEY (enablersid);


--
-- Name: enablers enablers_tripleidunique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablers_tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: ingredientcategory ing_cat_unique_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ing_cat_unique_name UNIQUE (name);


--
-- Name: ingredient ing_unique_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ing_unique_name UNIQUE (name);


--
-- Name: ingredientdecrement ingredient_decrement_unique_ordit_ingid; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_decrement_unique_ordit_ingid UNIQUE (orderitemid, ingredientid);


--
-- Name: ingredient ingredient_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredient_pkey PRIMARY KEY (ingredientid);


--
-- Name: ingredientcategory ingredientcateogory_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcategory
    ADD CONSTRAINT ingredientcateogory_pkey PRIMARY KEY (ingredientcategoryid);


--
-- Name: ingredientcourse ingredientcourse_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_pkey PRIMARY KEY (ingredientcourseid);


--
-- Name: ingredientdecrement ingredientincrement_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredientincrement_key PRIMARY KEY (ingredientdecrementid);


--
-- Name: ingredientincrement ingredientincrementid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT ingredientincrementid_key PRIMARY KEY (ingredientincrementid);


--
-- Name: ingredientprice ingredientprice_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_pkey PRIMARY KEY (ingredientpriceid);


--
-- Name: ingredientprice ingredientprice_uniq; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_uniq UNIQUE (ingredientid, quantity);


--
-- Name: invoices invoices_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoices_pkey PRIMARY KEY (invoicesid);


--
-- Name: orderitemsubordermapping mapping_orderitem_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemsubordermapping
    ADD CONSTRAINT mapping_orderitem_unique UNIQUE (orderitemid);


--
-- Name: observers observers_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT observers_pkey PRIMARY KEY (observersid);


--
-- Name: variations orderitem_ingredient_uniq; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT orderitem_ingredient_uniq UNIQUE (orderitemid, ingredientid);


--
-- Name: orderitems orderitems_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitems_pkey PRIMARY KEY (orderitemid);


--
-- Name: orderitemstates orderitemstates_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitemstates_pkey PRIMARY KEY (orderitemstatesid);


--
-- Name: orderitemsubordermapping orderitemsubordermapping_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemsubordermapping
    ADD CONSTRAINT orderitemsubordermapping_key PRIMARY KEY (orderitemsubordermappingid);


--
-- Name: orderoutgroup orderoutgroup_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_key PRIMARY KEY (ordergroupid);


--
-- Name: orders orders_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT orders_pkey PRIMARY KEY (orderid);


--
-- Name: paymentitem payment_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT payment_key PRIMARY KEY (paymentid);


--
-- Name: printerforcategory print_cat_state_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_cat_state_unique UNIQUE (categoryid, printerid, stateid);


--
-- Name: printers printer_unique_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printer_unique_name UNIQUE (name);


--
-- Name: printerforcategory printerforcategory_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printerforcategory_key PRIMARY KEY (printerforcategoryid);


--
-- Name: printerforreceiptandinvoice printerforreceiptandinvoice_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT printerforreceiptandinvoice_pkey PRIMARY KEY (printerforcategoryid);


--
-- Name: printers printers_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printers
    ADD CONSTRAINT printers_key PRIMARY KEY (printerid);


--
-- Name: rejectedorderitems rejecteddorderitemhistoryid_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejecteddorderitemhistoryid_pk PRIMARY KEY (rejectedorderitemid);


--
-- Name: roles roles_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_pkey PRIMARY KEY (roleid);


--
-- Name: roles roles_rolename_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roles
    ADD CONSTRAINT roles_rolename_key UNIQUE (rolename);


--
-- Name: standardvariationforcourse standard_variation_for_course_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationforcourse
    ADD CONSTRAINT standard_variation_for_course_pk PRIMARY KEY (standardvariationforcourseid);


--
-- Name: standardvariationitem standard_variation_item_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationitem
    ADD CONSTRAINT standard_variation_item_pk PRIMARY KEY (standardvariationitemid);


--
-- Name: standardvariations standard_variation_pk; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariations
    ADD CONSTRAINT standard_variation_pk PRIMARY KEY (standardvariationid);


--
-- Name: standardvariationforcourse standard_variation_unique_mapping; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationforcourse
    ADD CONSTRAINT standard_variation_unique_mapping UNIQUE (standardvariationid, courseid);


--
-- Name: standardvariations standard_variation_unique_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariations
    ADD CONSTRAINT standard_variation_unique_name UNIQUE (name);


--
-- Name: standardcomments standardcomment_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardcomments
    ADD CONSTRAINT standardcomment_pkey PRIMARY KEY (standardcommentid);


--
-- Name: states state_uniquename; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT state_uniquename UNIQUE (statusname);


--
-- Name: states stateid_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT stateid_pkey PRIMARY KEY (stateid);


--
-- Name: states states_statusname_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.states
    ADD CONSTRAINT states_statusname_key UNIQUE (statusname);


--
-- Name: subcategorymapping subcategorymapping_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT subcategorymapping_pkey PRIMARY KEY (subcategorymappingid);


--
-- Name: subcategorymapping subcategorymappingimentid_pair; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT subcategorymappingimentid_pair UNIQUE (fatherid, sonid);


--
-- Name: orderitemsubordermapping suborder_orderitem_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemsubordermapping
    ADD CONSTRAINT suborder_orderitem_unique UNIQUE (suborderid, orderitemid);


--
-- Name: suborder suborderid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderid_key PRIMARY KEY (suborderid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_pkey PRIMARY KEY (tempmuseractionablestatesid);


--
-- Name: temp_user_actionable_states temp_user_actionable_states_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_pkey PRIMARY KEY (tempuseractionablestateid);


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_unique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_unique UNIQUE (stateid);


--
-- Name: temp_user_actionable_states tempuserstateidunique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuserstateidunique UNIQUE (stateid, userid);


--
-- Name: tendercodes tendercode_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercode_key PRIMARY KEY (tendercodesid);


--
-- Name: tendercodes tendercodes_unique_code; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_code UNIQUE (tendercode);


--
-- Name: tendercodes tendercodes_unique_name; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.tendercodes
    ADD CONSTRAINT tendercodes_unique_name UNIQUE (tendername);


--
-- Name: observers tripleidunique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT tripleidunique UNIQUE (stateid, roleid, categoryid);


--
-- Name: orderoutgroup unique_group_order; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT unique_group_order UNIQUE (orderid, groupidentifier);


--
-- Name: ingredientcourse unique_ing_courrse; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT unique_ing_courrse UNIQUE (courseid, ingredientid);


--
-- Name: courses uniquename; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT uniquename UNIQUE (name);


--
-- Name: users users_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT users_pkey PRIMARY KEY (userid);


--
-- Name: variations variations_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variations_pkey PRIMARY KEY (variationsid);


--
-- Name: voidedorderslogbuffer voided_ord_uniq_user_order; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_uniq_user_order UNIQUE (userid, orderid);


--
-- Name: voidedorderslogbuffer voidedorderslogbufferid_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voidedorderslogbufferid_key PRIMARY KEY (voidedorderslogbufferid);


--
-- Name: waiteractionablestates waiteractionablestates_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiteractionablestates_pkey PRIMARY KEY (waiterwatchablestatesid);


--
-- Name: waiteractionablestates waiterdstateidunique; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterdstateidunique UNIQUE (stateid, userid);


--
-- Name: variations category_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT category_fk FOREIGN KEY (ingredientpriceid) REFERENCES public.ingredientprice(ingredientpriceid) MATCH FULL ON DELETE CASCADE;


--
-- Name: observers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: enablers category_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: printerforcategory category_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT category_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL ON DELETE CASCADE;


--
-- Name: courses categoryfk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.courses
    ADD CONSTRAINT categoryfk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: commentsforcourse commentsforcourse_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT commentsforcourse_fk FOREIGN KEY (standardcommentid) REFERENCES public.standardcomments(standardcommentid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitems coursefk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT coursefk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientincrement decrement_ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT decrement_ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL ON DELETE CASCADE;


--
-- Name: defaultactionablestates defaultactionablestates_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.defaultactionablestates
    ADD CONSTRAINT defaultactionablestates_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: enablers enablersrole; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT enablersrole FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL ON DELETE CASCADE;


--
-- Name: subcategorymapping father_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT father_fk FOREIGN KEY (fatherid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientdecrement ingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT ingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredient ingredientcategory_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredient
    ADD CONSTRAINT ingredientcategory_fk FOREIGN KEY (ingredientcategoryid) REFERENCES public.ingredientcategory(ingredientcategoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientcourse ingredientcourse_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientcourse ingredientcourse_ingredient_fk2; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientcourse
    ADD CONSTRAINT ingredientcourse_ingredient_fk2 FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientprice ingredientprice_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientprice
    ADD CONSTRAINT ingredientprice_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL ON DELETE CASCADE;


--
-- Name: invoices invoice_customer_data_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT invoice_customer_data_fk FOREIGN KEY (customerdataid) REFERENCES public.customerdata(customerdataid) MATCH FULL;


--
-- Name: printerforreceiptandinvoice invoice_receipt_printer_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforreceiptandinvoice
    ADD CONSTRAINT invoice_receipt_printer_fk FOREIGN KEY (printerid) REFERENCES public.printers(printerid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitemsubordermapping mapping_orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemsubordermapping
    ADD CONSTRAINT mapping_orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitemsubordermapping mapping_suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemsubordermapping
    ADD CONSTRAINT mapping_suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: paymentitem oorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT oorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: archivedorderslogbuffer order_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.archivedorderslogbuffer
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: invoices order_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitems orderdetail_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderdetail_fk FOREIGN KEY (ordergroupid) REFERENCES public.orderoutgroup(ordergroupid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitems orderfk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderfk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitemstates orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientdecrement orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitems orderitemstatus; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitems
    ADD CONSTRAINT orderitemstatus FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: orderoutgroup orderoutgroup_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderoutgroup
    ADD CONSTRAINT orderoutgroup_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: ingredientdecrement preparator_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientdecrement
    ADD CONSTRAINT preparator_fk FOREIGN KEY (preparatorid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: printerforcategory print_category_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT print_category_fk FOREIGN KEY (categoryid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: printerforcategory printer_cat_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.printerforcategory
    ADD CONSTRAINT printer_cat_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: rejectedorderitems reject_orderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT reject_orderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: rejectedorderitems rejected_item_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rejectedorderitems
    ADD CONSTRAINT rejected_item_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: observers role1_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT role1_fk FOREIGN KEY (roleid) REFERENCES public.roles(roleid) MATCH FULL ON DELETE CASCADE;


--
-- Name: users rolefk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.users
    ADD CONSTRAINT rolefk FOREIGN KEY (role) REFERENCES public.roles(roleid) MATCH FULL ON DELETE CASCADE;


--
-- Name: subcategorymapping son_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.subcategorymapping
    ADD CONSTRAINT son_fk FOREIGN KEY (sonid) REFERENCES public.coursecategories(categoryid) MATCH FULL ON DELETE CASCADE;


--
-- Name: standardvariationforcourse standard_variation_for_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationforcourse
    ADD CONSTRAINT standard_variation_for_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


--
-- Name: standardvariationitem standardvariation_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationitem
    ADD CONSTRAINT standardvariation_fk FOREIGN KEY (standardvariationid) REFERENCES public.standardvariations(standardvariationid) MATCH FULL ON DELETE CASCADE;


--
-- Name: standardvariationforcourse standardvariation_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationforcourse
    ADD CONSTRAINT standardvariation_fk FOREIGN KEY (standardvariationid) REFERENCES public.standardvariations(standardvariationid) MATCH FULL ON DELETE CASCADE;


--
-- Name: standardvariationitem standardvariationitem_ing_price_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.standardvariationitem
    ADD CONSTRAINT standardvariationitem_ing_price_fk FOREIGN KEY (ingredientpriceid) REFERENCES public.ingredientprice(ingredientpriceid) MATCH FULL ON DELETE SET NULL;


--
-- Name: observers state1_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.observers
    ADD CONSTRAINT state1_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orderitemstates state2_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orderitemstates
    ADD CONSTRAINT state2_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL ON DELETE CASCADE;


--
-- Name: enablers state_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.enablers
    ADD CONSTRAINT state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: invoices suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.invoices
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: paymentitem suborder_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT suborder_fk FOREIGN KEY (suborderid) REFERENCES public.suborder(suborderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: suborder suborderorder_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.suborder
    ADD CONSTRAINT suborderorder_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: temp_user_default_actionable_states temp_user_actionable_states_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_default_actionable_states
    ADD CONSTRAINT temp_user_actionable_states_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL;


--
-- Name: temp_user_actionable_states tempuser_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.temp_user_actionable_states
    ADD CONSTRAINT tempuser_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: paymentitem tendercode_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.paymentitem
    ADD CONSTRAINT tendercode_fk FOREIGN KEY (tendercodesid) REFERENCES public.tendercodes(tendercodesid) MATCH FULL;


--
-- Name: ingredientincrement user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.ingredientincrement
    ADD CONSTRAINT user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: orders userfk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.orders
    ADD CONSTRAINT userfk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: variations variationingredient_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationingredient_fk FOREIGN KEY (ingredientid) REFERENCES public.ingredient(ingredientid) MATCH FULL ON DELETE CASCADE;


--
-- Name: variations variationorderitem_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.variations
    ADD CONSTRAINT variationorderitem_fk FOREIGN KEY (orderitemid) REFERENCES public.orderitems(orderitemid) MATCH FULL ON DELETE CASCADE;


--
-- Name: voidedorderslogbuffer voided_ord_user_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_ord_user_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: voidedorderslogbuffer voided_order_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.voidedorderslogbuffer
    ADD CONSTRAINT voided_order_fk FOREIGN KEY (orderid) REFERENCES public.orders(orderid) MATCH FULL ON DELETE CASCADE;


--
-- Name: waiteractionablestates waiterstate_state_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstate_state_fk FOREIGN KEY (stateid) REFERENCES public.states(stateid) MATCH FULL ON DELETE CASCADE;


--
-- Name: waiteractionablestates waiterstateuser_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.waiteractionablestates
    ADD CONSTRAINT waiterstateuser_fk FOREIGN KEY (userid) REFERENCES public.users(userid) MATCH FULL ON DELETE CASCADE;


--
-- Name: SCHEMA public; Type: ACL; Schema: -; Owner: postgres
--

REVOKE USAGE ON SCHEMA public FROM PUBLIC;
GRANT ALL ON SCHEMA public TO PUBLIC;


--
-- Name: TABLE archivedorderslogbuffer; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.archivedorderslogbuffer TO suave;


--
-- Name: SEQUENCE archivedorderslog_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.archivedorderslog_id_seq TO suave;


--
-- Name: SEQUENCE comments_for_course_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.comments_for_course_seq TO suave;


--
-- Name: TABLE commentsforcourse; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.commentsforcourse TO suave;


--
-- Name: SEQUENCE standard_comments_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_comments_seq TO suave;


--
-- Name: TABLE standardcomments; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardcomments TO suave;


--
-- Name: TABLE commentsforcoursedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.commentsforcoursedetails TO suave;


--
-- Name: TABLE coursecategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.coursecategories TO suave;


--
-- Name: TABLE courses; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.courses TO suave;


--
-- Name: TABLE coursedetails2; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.coursedetails2 TO suave;


--
-- Name: SEQUENCE courses_categoryid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.courses_categoryid_seq TO suave;


--
-- Name: SEQUENCE courses_courseid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.courses_courseid_seq TO suave;


--
-- Name: SEQUENCE customerdata_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.customerdata_id_seq TO suave;


--
-- Name: TABLE customerdata; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.customerdata TO suave;


--
-- Name: SEQUENCE defaulwaiteractionablestates_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.defaulwaiteractionablestates_seq TO suave;


--
-- Name: TABLE defaultactionablestates; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.defaultactionablestates TO suave;


--
-- Name: SEQUENCE enablers_elablersid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.enablers_elablersid_seq TO suave;


--
-- Name: TABLE enablers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.enablers TO suave;


--
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.roles TO suave;


--
-- Name: SEQUENCE states_stateid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.states_stateid_seq TO suave;


--
-- Name: TABLE states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.states TO suave;


--
-- Name: TABLE enablersrolestatuscategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.enablersrolestatuscategories TO suave;


--
-- Name: SEQUENCE subcategory_mapping_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.subcategory_mapping_seq TO suave;


--
-- Name: TABLE subcategorymapping; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.subcategorymapping TO suave;


--
-- Name: TABLE fathersoncategoriesdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.fathersoncategoriesdetails TO suave;


--
-- Name: SEQUENCE incredientdecrementid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.incredientdecrementid_seq TO suave;


--
-- Name: TABLE ingredient; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredient TO suave;


--
-- Name: TABLE ingredientcategory; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientcategory TO suave;


--
-- Name: SEQUENCE ingredient_categoryid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredient_categoryid_seq TO suave;


--
-- Name: TABLE ingredientcourse; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientcourse TO suave;


--
-- Name: SEQUENCE ingredientcourseid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientcourseid_seq TO suave;


--
-- Name: TABLE ingredientdecrement; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdecrement TO suave;


--
-- Name: TABLE orderitems; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitems TO suave;


--
-- Name: TABLE orders; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orders TO suave;


--
-- Name: TABLE ingredientdecrementview; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdecrementview TO suave;


--
-- Name: TABLE ingredientdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdetails TO suave;


--
-- Name: SEQUENCE ingredientid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientid_seq TO suave;


--
-- Name: SEQUENCE ingredientincrementid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientincrementid_seq TO suave;


--
-- Name: TABLE ingredientincrement; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientincrement TO suave;


--
-- Name: TABLE ingredientofcourses; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientofcourses TO suave;


--
-- Name: SEQUENCE ingredientpriceid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientpriceid_seq TO suave;


--
-- Name: TABLE ingredientprice; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientprice TO suave;


--
-- Name: TABLE ingredientpricedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientpricedetails TO suave;


--
-- Name: SEQUENCE invoicesid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.invoicesid_seq TO suave;


--
-- Name: TABLE invoices; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.invoices TO suave;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.users TO suave;


--
-- Name: TABLE nonarchivedorderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.nonarchivedorderdetails TO suave;


--
-- Name: TABLE nonemptyorderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.nonemptyorderdetails TO suave;


--
-- Name: SEQUENCE observers_observerid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.observers_observerid_seq TO suave;


--
-- Name: TABLE observers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.observers TO suave;


--
-- Name: SEQUENCE observers_observersid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.observers_observersid_seq TO suave;


--
-- Name: TABLE observersrolestatuscategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.observersrolestatuscategories TO suave;


--
-- Name: TABLE orderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderdetails TO suave;


--
-- Name: SEQUENCE orderitem_sub_order_mapping_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitem_sub_order_mapping_seq TO suave;


--
-- Name: TABLE orderitemsubordermapping; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemsubordermapping TO suave;


--
-- Name: SEQUENCE orderoutgroup_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderoutgroup_id_seq TO suave;


--
-- Name: TABLE orderoutgroup; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderoutgroup TO suave;


--
-- Name: SEQUENCE suborderid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.suborderid_seq TO suave;


--
-- Name: TABLE suborder; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.suborder TO suave;


--
-- Name: TABLE orderitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemdetails TO suave;


--
-- Name: SEQUENCE orderitems_orderitemid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitems_orderitemid_seq TO suave;


--
-- Name: TABLE orderitemstates; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemstates TO suave;


--
-- Name: SEQUENCE orderitemstates_orderitemstates_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitemstates_orderitemstates_id_seq TO suave;


--
-- Name: TABLE orderoutgroupdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderoutgroupdetails TO suave;


--
-- Name: SEQUENCE orders_orderid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orders_orderid_seq TO suave;


--
-- Name: SEQUENCE paymentid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.paymentid_seq TO suave;


--
-- Name: TABLE paymentitem; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.paymentitem TO suave;


--
-- Name: SEQUENCE tendercodesid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.tendercodesid_seq TO suave;


--
-- Name: TABLE tendercodes; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.tendercodes TO suave;


--
-- Name: TABLE paymentitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.paymentitemdetails TO suave;


--
-- Name: SEQUENCE printerforcategory_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printerforcategory_id_seq TO suave;


--
-- Name: TABLE printerforcategory; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforcategory TO suave;


--
-- Name: SEQUENCE printers_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printers_id_seq TO suave;


--
-- Name: TABLE printers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printers TO suave;


--
-- Name: TABLE printerforcategorydetail; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforcategorydetail TO suave;


--
-- Name: SEQUENCE printerforreceiptandinvoice_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printerforreceiptandinvoice_id_seq TO suave;


--
-- Name: TABLE printerforreceiptandinvoice; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforreceiptandinvoice TO suave;


--
-- Name: SEQUENCE rejectedorderitems_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.rejectedorderitems_id_seq TO suave;


--
-- Name: TABLE rejectedorderitems; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.rejectedorderitems TO suave;


--
-- Name: SEQUENCE roles_roleid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.roles_roleid_seq TO suave;


--
-- Name: TABLE standardvariationforcourse; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationforcourse TO suave;


--
-- Name: SEQUENCE standard_variation_for_course_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_for_course_id_seq TO suave;


--
-- Name: TABLE standardvariations; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariations TO suave;


--
-- Name: SEQUENCE standard_variation_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_id_seq TO suave;


--
-- Name: TABLE standardvariationitem; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationitem TO suave;


--
-- Name: SEQUENCE standard_variation_item_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_item_id_seq TO suave;


--
-- Name: TABLE standardvariationforcoursedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationforcoursedetails TO suave;


--
-- Name: TABLE standardvariationitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationitemdetails TO suave;


--
-- Name: SEQUENCE tempuseractionablestates_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.tempuseractionablestates_seq TO suave;


--
-- Name: TABLE temp_user_actionable_states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.temp_user_actionable_states TO suave;


--
-- Name: SEQUENCE temp_user_actionable_states_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.temp_user_actionable_states_seq TO suave;


--
-- Name: TABLE temp_user_default_actionable_states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.temp_user_default_actionable_states TO suave;


--
-- Name: SEQUENCE users_userid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.users_userid_seq TO suave;


--
-- Name: TABLE usersview; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.usersview TO suave;


--
-- Name: TABLE variations; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.variations TO suave;


--
-- Name: TABLE variationdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.variationdetails TO suave;


--
-- Name: SEQUENCE variations_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.variations_seq TO suave;


--
-- Name: TABLE voidedorderslogbuffer; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.voidedorderslogbuffer TO suave;


--
-- Name: SEQUENCE voidedorderslog_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.voidedorderslog_id_seq TO suave;


--
-- Name: SEQUENCE waiteractionablestates_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.waiteractionablestates_seq TO suave;


--
-- Name: TABLE waiteractionablestates; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.waiteractionablestates TO suave;


--
-- PostgreSQL database dump complete
--


--
-- PostgreSQL database dump
--

-- Dumped from database version 10.13
-- Dumped by pg_dump version 10.13

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
    courseid integer NOT NULL,
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

CREATE SEQUENCE public.courses_courseid_seq
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
-- Name: ingredientdecrementview; Type: VIEW; Schema: public; Owner: postgres
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
    (COALESCE(f.payed, false) OR d.archived) AS payed
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

CREATE SEQUENCE public.users_userid_seq
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
293	2020-12-12 16:52:08.669483	621
294	2020-12-27 18:27:17.02626	620
295	2020-12-27 18:30:50.040048	622
\.


--
-- Data for Name: commentsforcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.commentsforcourse (commentsforcourseid, courseid, standardcommentid) FROM stdin;
4	537	6
7	538	11
8	538	10
9	540	11
10	540	10
11	541	12
12	541	13
13	541	14
\.


--
-- Data for Name: coursecategories; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.coursecategories (categoryid, name, visibile, abstract) FROM stdin;
73	pranzo	t	t
70	secondi	t	f
71	superalcolici	t	f
75	primi	t	f
76	dessert	t	f
90	light	t	f
74	bevande	t	f
\.


--
-- Data for Name: courses; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.courses (courseid, name, description, price, categoryid, visibility) FROM stdin;
537	cuba libre		6.00	71	t
538	pranzo		10.00	73	f
540	bistecca di manzo		15.00	70	t
541	spaghetti allo scoglio		10.00	75	t
542	polpetta		2.00	70	t
543	gelato al cioccolato		4.00	76	t
544	coca cola		4.00	90	t
545	penne all'arrabiata		8.00	75	t
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
\.


--
-- Data for Name: ingredient; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredient (ingredientid, ingredientcategoryid, name, description, visibility, allergen, updateavailabilityflag, availablequantity, checkavailabilityflag, unitmeasure) FROM stdin;
181	57	havana 3		t	f	f	0.00	f	cl
182	57	havana 7		t	f	f	0.00	f	cl
183	58	coca cola		t	f	f	0.00	f	unit├á
185	58	fanta	classica aranciata	t	f	f	0.00	f	lt
186	59	manzo		t	f	f	0.00	f	unit├á
180	56	lattuga		t	f	f	0.00	f	gr
187	56	pomodoro		t	f	f	0.00	f	gr
184	58	pepsi		t	f	f	0.00	f	cl
188	56	sugo di pomodoro		t	f	f	0.00	f	cl
189	60	pennette rigate		t	f	f	0.00	f	gr
190	60	strozzapreti	classica fresca	t	t	f	0.00	f	gr
\.


--
-- Data for Name: ingredientcategory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientcategory (ingredientcategoryid, name, description, visibility) FROM stdin;
59	carni		t
58	soft drink		t
57	superalcolici		t
56	vegetables		t
60	pasta		t
61	pro		t
\.


--
-- Data for Name: ingredientcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientcourse (ingredientcourseid, courseid, ingredientid, quantity) FROM stdin;
363	537	181	\N
368	540	186	1.00
369	540	180	50.00
370	540	187	\N
372	541	180	\N
\.


--
-- Data for Name: ingredientdecrement; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientdecrement (ingredientdecrementid, orderitemid, typeofdecrement, presumednormalquantity, recordedquantity, preparatorid, registrationtime, ingredientid) FROM stdin;
275	1485	NORMAL	50.00	\N	2	2020-12-12 15:20:46.439099	180
276	1485	NORMAL	1.00	\N	2	2020-12-12 15:20:46.439099	186
284	1492	cl	1.00	\N	2	2020-12-20 14:36:48.009481	184
286	1498	NORMAL	50.00	\N	2	2020-12-20 14:56:18.181894	180
287	1498	NORMAL	1.00	\N	2	2020-12-20 14:56:18.181894	186
288	1502	gr	0.50	0.00	2	2020-12-20 14:36:48.040655	180
289	1503	gr	0.50	0.00	2	2020-12-20 14:36:48.040655	180
\.


--
-- Data for Name: ingredientincrement; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientincrement (ingredientincrementid, ingredientid, comment, unitofmeasure, quantity, userid, registrationtime) FROM stdin;
\.


--
-- Data for Name: ingredientprice; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.ingredientprice (ingredientpriceid, ingredientid, quantity, isdefaultadd, isdefaultsubtract, addprice, subtractprice) FROM stdin;
105	182	10.00	t	t	1.00	1.00
106	184	10.00	t	t	10.00	10.00
107	183	1.00	t	t	1.00	1.00
108	186	1.00	t	t	5.00	5.00
110	180	50.00	t	t	1.00	1.00
\.


--
-- Data for Name: invoices; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.invoices (invoicesid, data, invoicenumber, customerdataid, date, suborderid, orderid) FROM stdin;
\.


--
-- Data for Name: observers; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.observers (observersid, stateid, roleid, categoryid) FROM stdin;
\.


--
-- Data for Name: orderitems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitems (orderitemid, courseid, quantity, orderid, comment, price, stateid, archived, startingtime, closingtime, ordergroupid, hasbeenrejected, printcount) FROM stdin;
1490	537	1	622	, poco ghiaccio	6.00	2	\N	2020-12-12 16:52:42.342057	\N	574	f	0
1485	540	1	621	, ben cotta ma non tantissimo	15.00	2	\N	2020-12-12 15:16:45.657077	\N	571	f	0
1486	537	1	621	, poco ghiaccio	6.00	2	\N	2020-12-12 15:20:25.989476	\N	571	f	0
1491	537	1	622		6.00	2	\N	2020-12-12 16:53:07.702446	\N	574	f	0
1492	541	1	622	, piccante	10.00	2	\N	2020-12-12 16:53:17.61286	\N	574	f	0
1498	540	1	620	, ben cotta	15.00	2	\N	2020-12-17 21:32:54.45194	\N	577	f	0
1502	542	1	622		2.00	6	\N	2020-12-27 18:27:22.93189	\N	574	f	1
1503	542	1	622		2.00	6	\N	2020-12-27 18:27:22.937469	\N	574	f	1
1488	541	1	621	, piccante	10.00	1	\N	2020-12-12 15:30:24.735806	\N	573	f	0
1489	540	1	621		15.00	1	\N	2020-12-12 15:32:48.089333	\N	573	f	0
1505	541	1	625	, al dente, piccante	10.00	2	\N	2020-12-28 09:28:16.548954	\N	581	f	0
\.


--
-- Data for Name: orderitemstates; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitemstates (orderitemstatesid, orderitemid, stateid, startingtime) FROM stdin;
2749	1485	1	2020-12-12 15:16:45.657077
2750	1486	1	2020-12-12 15:20:25.989476
2751	1485	2	2020-12-12 15:20:46.422097
2752	1486	2	2020-12-12 15:20:46.497209
2754	1488	1	2020-12-12 15:30:24.735806
2755	1489	1	2020-12-12 15:32:48.089333
2756	1490	1	2020-12-12 16:52:42.342057
2757	1491	1	2020-12-12 16:53:07.702446
2758	1492	1	2020-12-12 16:53:17.61286
2766	1498	1	2020-12-17 21:32:54.45194
2774	1490	2	2020-12-20 14:36:47.949345
2775	1491	2	2020-12-20 14:36:47.989714
2776	1492	2	2020-12-20 14:36:48.004508
2778	1498	2	2020-12-20 14:56:18.105431
2779	1502	1	2020-12-17 20:56:07.771011
2780	1503	2	2020-12-20 14:36:48.036612
2781	1502	2	2020-12-20 14:36:48.036612
2782	1503	1	2020-12-17 20:56:07.771011
2784	1505	1	2020-12-28 09:28:16.548954
2785	1505	2	2020-12-28 09:30:31.435428
\.


--
-- Data for Name: orderitemsubordermapping; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderitemsubordermapping (orderitemsubordermappingid, orderitemid, suborderid) FROM stdin;
81	1486	393
82	1485	394
83	1490	395
84	1491	395
85	1492	395
86	1502	396
87	1503	396
\.


--
-- Data for Name: orderoutgroup; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orderoutgroup (ordergroupid, printcount, orderid, groupidentifier) FROM stdin;
571	1	621	1
573	0	621	2
567	1	620	1
574	1	622	1
577	1	620	2
581	1	625	1
\.


--
-- Data for Name: orders; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.orders (orderid, "table", person, ongoing, userid, startingtime, closingtime, voided, archived, total, adjustedtotal, plaintotalvariation, percentagevariataion, adjustispercentage, adjustisplain, forqruserarchived) FROM stdin;
621	7		t	2	2020-12-12 15:14:43.064389	2020-12-12 16:52:08.667352	f	t	21.00	21.00	0.00	0.00	f	f	\N
625	1		t	2	2020-12-28 09:22:52.242677	\N	f	f	10.00	10.00	0.00	0.00	f	f	\N
620	6		t	2	2020-08-02 14:46:27.838341	2020-12-27 18:27:17.021548	f	t	15.00	15.00	0.00	0.00	f	f	\N
622	9		t	2	2020-12-12 16:52:38.462428	2020-12-27 18:30:50.038504	f	t	26.00	26.00	0.00	0.00	f	f	\N
\.


--
-- Data for Name: paymentitem; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.paymentitem (paymentid, suborderid, orderid, tendercodesid, amount) FROM stdin;
150	393	\N	1	6.00
151	394	\N	1	15.00
152	\N	620	1	10.00
153	\N	620	1	5.00
154	396	\N	1	4.00
155	395	\N	1	22.00
\.


--
-- Data for Name: printerforcategory; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.printerforcategory (printerforcategoryid, categoryid, printerid, stateid) FROM stdin;
60	70	51	2
61	75	51	2
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
50	Fax
51	Generic
52	TOSHIBA e-STUDIO18 Printer
53	OneNote for Windows 10
54	OneNote (Desktop)
55	Microsoft XPS Document Writer
56	Microsoft Print to PDF
57	GenericTextOnly
58	Bullzip PDF Printer
\.


--
-- Data for Name: rejectedorderitems; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rejectedorderitems (rejectedorderitemid, courseid, cause, timeofrejection, orderitemid) FROM stdin;
\.


--
-- Data for Name: roles; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roles (roleid, rolename, comment) FROM stdin;
1	admin	\N
28	temporary	
29	cameriere	
\.


--
-- Data for Name: standardcomments; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardcomments (standardcommentid, comment) FROM stdin;
6	poco ghiaccio
9	molto ghiaccio
10	ben cotta
11	al sangue
12	piccante
13	al dente
14	cottura normale
\.


--
-- Data for Name: standardvariationforcourse; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariationforcourse (standardvariationforcourseid, standardvariationid, courseid) FROM stdin;
\.


--
-- Data for Name: standardvariationitem; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariationitem (standardvariationitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid, standardvariationid) FROM stdin;
5	182	­ƒæì	\N	\N	4
15	183	­ƒÜ½	\N	107	3
\.


--
-- Data for Name: standardvariations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.standardvariations (standardvariationid, name) FROM stdin;
3	coca cola
4	havana
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
3	74	90
\.


--
-- Data for Name: suborder; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.suborder (suborderid, orderid, subtotal, comment, payed, creationtime, tendercodesid, subtotaladjustment, subtotalpercentadjustment) FROM stdin;
393	621	6.00	\N	t	2020-12-12 15:22:49.654986	\N	0.00	0.00
394	621	15.00	\N	t	2020-12-12 15:23:00.029612	\N	0.00	0.00
396	622	4.00	\N	t	2020-12-27 18:27:35.729717	\N	0.00	0.00
395	622	22.00	\N	t	2020-12-27 18:27:30.590288	\N	0.00	0.00
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
\.


--
-- Data for Name: variations; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.variations (variationsid, orderitemid, ingredientid, tipovariazione, plailnumvariation, ingredientpriceid) FROM stdin;
1149	1485	187	­ƒÜ½	\N	\N
1151	1488	180	gr	1	\N
1152	1492	184	cl	1	\N
1161	1502	180	gr	\N	\N
1162	1503	180	gr	\N	\N
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

SELECT pg_catalog.setval('public.archivedorderslog_id_seq', 295, true);


--
-- Name: comments_for_course_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.comments_for_course_seq', 13, true);


--
-- Name: courses_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.courses_categoryid_seq', 90, true);


--
-- Name: courses_courseid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.courses_courseid_seq', 545, true);


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

SELECT pg_catalog.setval('public.enablers_elablersid_seq', 190, true);


--
-- Name: incredientdecrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.incredientdecrementid_seq', 289, true);


--
-- Name: ingredient_categoryid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredient_categoryid_seq', 61, true);


--
-- Name: ingredientcourseid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientcourseid_seq', 372, true);


--
-- Name: ingredientid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientid_seq', 190, true);


--
-- Name: ingredientincrementid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientincrementid_seq', 32, true);


--
-- Name: ingredientpriceid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.ingredientpriceid_seq', 110, true);


--
-- Name: invoicesid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.invoicesid_seq', 45, true);


--
-- Name: observers_observerid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.observers_observerid_seq', 210, true);


--
-- Name: observers_observersid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.observers_observersid_seq', 1, false);


--
-- Name: orderitem_sub_order_mapping_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitem_sub_order_mapping_seq', 89, true);


--
-- Name: orderitems_orderitemid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitems_orderitemid_seq', 1505, true);


--
-- Name: orderitemstates_orderitemstates_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderitemstates_orderitemstates_id_seq', 2785, true);


--
-- Name: orderoutgroup_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orderoutgroup_id_seq', 581, true);


--
-- Name: orders_orderid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.orders_orderid_seq', 625, true);


--
-- Name: paymentid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.paymentid_seq', 164, true);


--
-- Name: printerforcategory_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printerforcategory_id_seq', 61, true);


--
-- Name: printerforreceiptandinvoice_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printerforreceiptandinvoice_id_seq', 4, true);


--
-- Name: printers_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.printers_id_seq', 58, true);


--
-- Name: rejectedorderitems_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.rejectedorderitems_id_seq', 81, true);


--
-- Name: roles_roleid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roles_roleid_seq', 29, true);


--
-- Name: standard_comments_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_comments_seq', 14, true);


--
-- Name: standard_variation_for_course_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_for_course_id_seq', 3, true);


--
-- Name: standard_variation_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_id_seq', 5, true);


--
-- Name: standard_variation_item_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.standard_variation_item_id_seq', 15, true);


--
-- Name: states_stateid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.states_stateid_seq', 6, true);


--
-- Name: subcategory_mapping_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.subcategory_mapping_seq', 3, true);


--
-- Name: suborderid_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.suborderid_seq', 397, true);


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

SELECT pg_catalog.setval('public.users_userid_seq', 183, true);


--
-- Name: variations_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.variations_seq', 1162, true);


--
-- Name: voidedorderslog_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.voidedorderslog_id_seq', 317, true);


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
-- Name: commentsforcourse commentsforcourse_course_fk; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.commentsforcourse
    ADD CONSTRAINT commentsforcourse_course_fk FOREIGN KEY (courseid) REFERENCES public.courses(courseid) MATCH FULL ON DELETE CASCADE;


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

GRANT USAGE ON SCHEMA public TO compraga_app;


--
-- Name: TABLE archivedorderslogbuffer; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.archivedorderslogbuffer TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.archivedorderslogbuffer TO compraga_app;


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
GRANT SELECT,INSERT,UPDATE ON TABLE public.commentsforcourse TO compraga_app;


--
-- Name: SEQUENCE standard_comments_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_comments_seq TO suave;


--
-- Name: TABLE standardcomments; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardcomments TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardcomments TO compraga_app;


--
-- Name: TABLE commentsforcoursedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.commentsforcoursedetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.commentsforcoursedetails TO compraga_app;


--
-- Name: TABLE coursecategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.coursecategories TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.coursecategories TO compraga_app;


--
-- Name: TABLE courses; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.courses TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.courses TO compraga_app;


--
-- Name: TABLE coursedetails2; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.coursedetails2 TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.coursedetails2 TO compraga_app;


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
GRANT SELECT,INSERT,UPDATE ON TABLE public.customerdata TO compraga_app;


--
-- Name: SEQUENCE defaulwaiteractionablestates_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.defaulwaiteractionablestates_seq TO suave;


--
-- Name: TABLE defaultactionablestates; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.defaultactionablestates TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.defaultactionablestates TO compraga_app;


--
-- Name: SEQUENCE enablers_elablersid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.enablers_elablersid_seq TO suave;


--
-- Name: TABLE enablers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.enablers TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.enablers TO compraga_app;


--
-- Name: TABLE roles; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.roles TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.roles TO compraga_app;


--
-- Name: SEQUENCE states_stateid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.states_stateid_seq TO suave;


--
-- Name: TABLE states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.states TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.states TO compraga_app;


--
-- Name: TABLE enablersrolestatuscategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.enablersrolestatuscategories TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.enablersrolestatuscategories TO compraga_app;


--
-- Name: SEQUENCE subcategory_mapping_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.subcategory_mapping_seq TO suave;


--
-- Name: TABLE subcategorymapping; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.subcategorymapping TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.subcategorymapping TO compraga_app;


--
-- Name: TABLE fathersoncategoriesdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.fathersoncategoriesdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.fathersoncategoriesdetails TO compraga_app;


--
-- Name: SEQUENCE incredientdecrementid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.incredientdecrementid_seq TO suave;


--
-- Name: TABLE ingredient; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredient TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredient TO compraga_app;


--
-- Name: TABLE ingredientcategory; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientcategory TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientcategory TO compraga_app;


--
-- Name: SEQUENCE ingredient_categoryid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredient_categoryid_seq TO suave;


--
-- Name: TABLE ingredientcourse; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientcourse TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientcourse TO compraga_app;


--
-- Name: SEQUENCE ingredientcourseid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientcourseid_seq TO suave;


--
-- Name: TABLE ingredientdecrement; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdecrement TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientdecrement TO compraga_app;


--
-- Name: TABLE orderitems; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitems TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderitems TO compraga_app;


--
-- Name: TABLE ingredientdecrementview; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdecrementview TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientdecrementview TO compraga_app;


--
-- Name: TABLE ingredientdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientdetails TO compraga_app;


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
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientincrement TO compraga_app;


--
-- Name: TABLE ingredientofcourses; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientofcourses TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientofcourses TO compraga_app;


--
-- Name: SEQUENCE ingredientpriceid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.ingredientpriceid_seq TO suave;


--
-- Name: TABLE ingredientprice; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientprice TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientprice TO compraga_app;


--
-- Name: TABLE ingredientpricedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.ingredientpricedetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.ingredientpricedetails TO compraga_app;


--
-- Name: SEQUENCE invoicesid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.invoicesid_seq TO suave;


--
-- Name: TABLE invoices; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.invoices TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.invoices TO compraga_app;


--
-- Name: TABLE orders; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orders TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orders TO compraga_app;


--
-- Name: TABLE users; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.users TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.users TO compraga_app;


--
-- Name: TABLE nonarchivedorderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.nonarchivedorderdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.nonarchivedorderdetails TO compraga_app;


--
-- Name: TABLE nonemptyorderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.nonemptyorderdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.nonemptyorderdetails TO compraga_app;


--
-- Name: SEQUENCE observers_observerid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.observers_observerid_seq TO suave;


--
-- Name: TABLE observers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.observers TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.observers TO compraga_app;


--
-- Name: SEQUENCE observers_observersid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.observers_observersid_seq TO suave;


--
-- Name: TABLE observersrolestatuscategories; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.observersrolestatuscategories TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.observersrolestatuscategories TO compraga_app;


--
-- Name: TABLE orderdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderdetails TO compraga_app;


--
-- Name: SEQUENCE orderitem_sub_order_mapping_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitem_sub_order_mapping_seq TO suave;


--
-- Name: TABLE orderitemsubordermapping; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemsubordermapping TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderitemsubordermapping TO compraga_app;


--
-- Name: SEQUENCE orderoutgroup_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderoutgroup_id_seq TO suave;


--
-- Name: TABLE orderoutgroup; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderoutgroup TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderoutgroup TO compraga_app;


--
-- Name: SEQUENCE suborderid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.suborderid_seq TO suave;


--
-- Name: TABLE suborder; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.suborder TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.suborder TO compraga_app;


--
-- Name: TABLE orderitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderitemdetails TO compraga_app;


--
-- Name: SEQUENCE orderitems_orderitemid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitems_orderitemid_seq TO suave;


--
-- Name: TABLE orderitemstates; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderitemstates TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderitemstates TO compraga_app;


--
-- Name: SEQUENCE orderitemstates_orderitemstates_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.orderitemstates_orderitemstates_id_seq TO suave;


--
-- Name: TABLE orderoutgroupdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.orderoutgroupdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.orderoutgroupdetails TO compraga_app;


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
GRANT SELECT,INSERT,UPDATE ON TABLE public.paymentitem TO compraga_app;


--
-- Name: SEQUENCE tendercodesid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.tendercodesid_seq TO suave;


--
-- Name: TABLE tendercodes; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.tendercodes TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.tendercodes TO compraga_app;


--
-- Name: TABLE paymentitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.paymentitemdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.paymentitemdetails TO compraga_app;


--
-- Name: SEQUENCE printerforcategory_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printerforcategory_id_seq TO suave;


--
-- Name: TABLE printerforcategory; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforcategory TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.printerforcategory TO compraga_app;


--
-- Name: SEQUENCE printers_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printers_id_seq TO suave;


--
-- Name: TABLE printers; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printers TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.printers TO compraga_app;


--
-- Name: TABLE printerforcategorydetail; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforcategorydetail TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.printerforcategorydetail TO compraga_app;


--
-- Name: SEQUENCE printerforreceiptandinvoice_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.printerforreceiptandinvoice_id_seq TO suave;


--
-- Name: TABLE printerforreceiptandinvoice; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.printerforreceiptandinvoice TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.printerforreceiptandinvoice TO compraga_app;


--
-- Name: SEQUENCE rejectedorderitems_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.rejectedorderitems_id_seq TO suave;


--
-- Name: TABLE rejectedorderitems; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.rejectedorderitems TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.rejectedorderitems TO compraga_app;


--
-- Name: SEQUENCE roles_roleid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.roles_roleid_seq TO suave;


--
-- Name: TABLE standardvariationforcourse; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationforcourse TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardvariationforcourse TO compraga_app;


--
-- Name: SEQUENCE standard_variation_for_course_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_for_course_id_seq TO suave;


--
-- Name: TABLE standardvariations; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariations TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardvariations TO compraga_app;


--
-- Name: SEQUENCE standard_variation_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_id_seq TO suave;


--
-- Name: TABLE standardvariationitem; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationitem TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardvariationitem TO compraga_app;


--
-- Name: SEQUENCE standard_variation_item_id_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.standard_variation_item_id_seq TO suave;


--
-- Name: TABLE standardvariationforcoursedetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationforcoursedetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardvariationforcoursedetails TO compraga_app;


--
-- Name: TABLE standardvariationitemdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.standardvariationitemdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.standardvariationitemdetails TO compraga_app;


--
-- Name: SEQUENCE tempuseractionablestates_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.tempuseractionablestates_seq TO suave;


--
-- Name: TABLE temp_user_actionable_states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.temp_user_actionable_states TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.temp_user_actionable_states TO compraga_app;


--
-- Name: SEQUENCE temp_user_actionable_states_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.temp_user_actionable_states_seq TO suave;


--
-- Name: TABLE temp_user_default_actionable_states; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.temp_user_default_actionable_states TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.temp_user_default_actionable_states TO compraga_app;


--
-- Name: SEQUENCE users_userid_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.users_userid_seq TO suave;


--
-- Name: TABLE usersview; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.usersview TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.usersview TO compraga_app;


--
-- Name: TABLE variations; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.variations TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.variations TO compraga_app;


--
-- Name: TABLE variationdetails; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.variationdetails TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.variationdetails TO compraga_app;


--
-- Name: SEQUENCE variations_seq; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON SEQUENCE public.variations_seq TO suave;


--
-- Name: TABLE voidedorderslogbuffer; Type: ACL; Schema: public; Owner: postgres
--

GRANT ALL ON TABLE public.voidedorderslogbuffer TO suave;
GRANT SELECT,INSERT,UPDATE ON TABLE public.voidedorderslogbuffer TO compraga_app;


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
GRANT SELECT,INSERT,UPDATE ON TABLE public.waiteractionablestates TO compraga_app;


--
-- PostgreSQL database dump complete
--


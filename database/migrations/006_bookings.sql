BEGIN;

CREATE SEQUENCE IF NOT EXISTS airportdb.booking_booking_id_seq;

SELECT setval(
    'airportdb.booking_booking_id_seq',
    GREATEST((SELECT COALESCE(MAX(booking_id), 0) FROM airportdb.booking), 1),
    true);

ALTER SEQUENCE airportdb.booking_booking_id_seq
    OWNED BY airportdb.booking.booking_id;

ALTER TABLE airportdb.booking
    ALTER COLUMN booking_id
    SET DEFAULT nextval('airportdb.booking_booking_id_seq');

CREATE TABLE IF NOT EXISTS airportdb.booking_cancellation
(
    booking_id integer PRIMARY KEY
        REFERENCES airportdb.booking (booking_id),
    cancelled_at timestamp with time zone NOT NULL,
    cancelled_by integer NOT NULL
        REFERENCES airportdb.employee (employee_id),
    reason character varying(250) NOT NULL,
    CONSTRAINT booking_cancellation_reason_chk
        CHECK (length(btrim(reason)) BETWEEN 3 AND 250)
);

CREATE INDEX IF NOT EXISTS idx_booking_cancellation_cancelled_at
    ON airportdb.booking_cancellation (cancelled_at);

COMMIT;
